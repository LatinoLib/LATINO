using System;
using System.Collections.Generic;

namespace Latino.Model
{
    /*
     * Copyright (C) 2007 by
     * 
     * 	Xuan-Hieu Phan
     *	hieuxuan@ecei.tohoku.ac.jp or pxhieu@gmail.com
     * 	Graduate School of Information Sciences
     * 	Tohoku University
     *
     * GibbsLDA++ is a free software; you can redistribute it and/or modify
     * it under the terms of the GNU General Public License as published
     * by the Free Software Foundation; either version 2 of the License,
     * or (at your option) any later version.
     *
     * GibbsLDA++ is distributed in the hope that it will be useful, but
     * WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
     * GNU General Public License for more details.
     *
     * You should have received a copy of the GNU General Public License
     * along with GibbsLDA++; if not, write to the Free Software Foundation,
     * Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA.
     */

    /* 
     * References:
     * + The Java code of Gregor Heinrich (gregor@arbylon.net)
     *   http://www.arbylon.net/projects/LdaGibbsSampler.java
     * + "Parameter estimation for text analysis" by Gregor Heinrich
     *   http://www.arbylon.net/publications/text-est.pdf
     */

    // map of words/terms [string => int]
//    typedef map<string, int> mapword2id;
    // map of words/terms [int => string]
//    typedef map<int, string> mapid2word;

    public class document
    {
        public int[] words;
        public string rawstr;
        public int length;

        public document()
        {
            words = null;
            rawstr = "";
            length = 0;
        }

        public document(int length)
        {
            this.length = length;
            rawstr = "";
            words = new int[length];
        }

        public document(int length, int[] words)
        {
            this.length = length;
            rawstr = "";
            this.words = new int[length];
            for (int i = 0; i < length; i++)
            {
                this.words[i] = words[i];
            }
        }

        public document(int length, int[] words, string rawstr)
        {
            this.length = length;
            this.rawstr = rawstr;
            this.words = new int[length];
            for (int i = 0; i < length; i++)
            {
                this.words[i] = words[i];
            }
        }
    };

    public class dataset
    {
        public document[] docs;
        public document[] _docs; // used only for inference
        public Dictionary<int, int> _id2id; // also used only for inference
        public int M; // number of documents
        public int V; // number of words

        public dataset()
        {
            docs = null;
            _docs = null;
            M = 0;
            V = 0;
        }

        public dataset(int M)
        {
            this.M = M;
            this.V = 0;
            docs = new document[M];
            _docs = null;
        }

        public void add_doc(document doc, int idx)
        {
            if (0 <= idx && idx < M)
            {
                docs[idx] = doc;
            }
        }

        public void _add_doc(document doc, int idx)
        {
            if (0 <= idx && idx < M)
            {
                _docs[idx] = doc;
            }
        }

        public static int write_wordmap(string wordmapfile, Dictionary<string, int> pword2id)
        {
            return 0;
        }

        public static int read_wordmap(string wordmapfile, Dictionary<string, int> pword2id)
        {
            return 0;
        }

        public static int read_wordmap(string wordmapfile, Dictionary<int, string> pid2word)
        {
            return 0;
        }

        public bool read_trndata(string dfile, string wordmapfile)
        {
            return true;
        }

        public bool read_newdata(string dfile, string wordmapfile)
        {
            return true;
        }

        public bool read_newdata_withrawstrs(string dfile, string wordmapfile)
        {
            return true;
        }
    }


    public class model
    {
        const int MODEL_STATUS_UNKNOWN = 0;
        const int MODEL_STATUS_EST = 1;
        const int MODEL_STATUS_ESTC = 2;
        const int MODEL_STATUS_INF = 3;

        // fixed options
        string wordmapfile;     // file that contains word map [string . integer id]
        string trainlogfile;    // training log file
        string tassign_suffix;  // suffix for topic assignment file
        string theta_suffix;    // suffix for theta file
        string phi_suffix;      // suffix for phi file
        string others_suffix;   // suffix for file containing other parameters
        string twords_suffix;   // suffix for file containing words-per-topics

        string dir;         // model directory
        string dfile;       // data file    
        string model_name;      // model name
        int model_status;       // model status:
                                // MODEL_STATUS_UNKNOWN: unknown status
                                // MODEL_STATUS_EST: estimating from scratch
                                // MODEL_STATUS_ESTC: continue to estimate the model from a previous one
                                // MODEL_STATUS_INF: do inference

        dataset ptrndata;  // pointer to training dataset object
        dataset pnewdata; // pointer to new dataset object

        Dictionary<int, string> id2word; // word map [int => string]

        // --- model parameters and variables ---    
        int M; // dataset size (i.e., number of docs)
        int V; // vocabulary size
        int K; // number of topics
        double alpha, beta; // LDA hyperparameters 
        int niters; // number of Gibbs sampling iterations
        int liter; // the iteration at which the model was saved
        int savestep; // saving period
        int twords; // print out top words per each topic
        bool withrawstrs;

        double[] p; // temp variable for sampling
        int[][] z; // topic assignments for words, size M x doc.size()
        int[][] nw; // cwt[i][j]: number of instances of word/term i assigned to topic j, size V x K
        int[][] nd; // na[i][j]: number of words in document i assigned to topic j, size M x K
        int[] nwsum; // nwsum[j]: total number of words assigned to topic j, size K
        int[] ndsum; // nasum[i]: total number of words in document i, size M
        double[][] theta; // theta: document-topic distributions, size M x K
        double[][] phi; // phi: topic-word distributions, size K x V

        // for inference only
        int inf_liter;
        int newM;
        int newV;
        int[][] newz;
        int[][] newnw;
        int[][] newnd;
        int[] newnwsum;
        int[] newndsum;
        double[][] newtheta;
        double[][] newphi;
        // --------------------------------------

        model()
        {
            set_default_values();
        }
        

        // set default values for variables
        void set_default_values()
        {
            wordmapfile = "wordmap.txt";
            trainlogfile = "trainlog.txt";
            tassign_suffix = ".tassign";
            theta_suffix = ".theta";
            phi_suffix = ".phi";
            others_suffix = ".others";
            twords_suffix = ".twords";

            dir = "./";
            dfile = "trndocs.dat";
            model_name = "model-final";
            model_status = MODEL_STATUS_UNKNOWN;

            ptrndata = null;
            pnewdata = null;

            M = 0;
            V = 0;
            K = 100;
            alpha = 50.0 / K;
            beta = 0.1;
            niters = 2000;
            liter = 0;
            savestep = 200;
            twords = 0;
            withrawstrs = false;

            p = null;
            z = null;
            nw = null;
            nd = null;
            nwsum = null;
            ndsum = null;
//            theta = null;
//            phi = null;

            newM = 0;
            newV = 0;
//            newz = null;
//            newnw = null;
//            newnd = null;
//            newnwsum = null;
//            newndsum = null;
//            newtheta = null;
//            newphi = null;
        }



        // initialize the model
        bool init(int argc, char argv)
        {

            if (model_status == MODEL_STATUS_EST)
            {
                // estimating the model from scratch
                if (init_est())
                {
                    return false;
                }

            }
            else if (model_status == MODEL_STATUS_ESTC)
            {
                // estimating the model from a previously estimated one
                if (init_estc())
                {
                    return false;
                }

            }
            else if (model_status == MODEL_STATUS_INF)
            {
                // do inference
                if (init_inf())
                {
                    return false;
                }
            }

            return true;
        }


        // init for estimation
        bool init_est()
        {
            int m, n, w, k;

            p = new double[K];

            // + read training data
            ptrndata = new dataset();
//            if (ptrndata.read_trndata(dir + dfile, dir + wordmapfile))
//            {
//                printf("Fail to read training data!\n");
//                return 1;
//            }

            // + allocate memory and assign values for variables
            M = ptrndata.M;
            V = ptrndata.V;
            // K: from command line or default value
            // alpha, beta: from command line or default values
            // niters, savestep: from command line or default values

            nw = new int[V][];
            for (w = 0; w < V; w++)
            {
                nw[w] = new int[K];
                for (k = 0; k < K; k++)
                {
                    nw[w][k] = 0;
                }
            }

            nd = new int[M][];
            for (m = 0; m < M; m++)
            {
                nd[m] = new int[K];
                for (k = 0; k < K; k++)
                {
                    nd[m][k] = 0;
                }
            }

            nwsum = new int[K];
            for (k = 0; k < K; k++)
            {
                nwsum[k] = 0;
            }

            ndsum = new int[M];
            for (m = 0; m < M; m++)
            {
                ndsum[m] = 0;
            }

            var random = new Random();
//            srandom(time(0)); // initialize for random number generation
            z = new int[M][];
            for (m = 0; m < ptrndata.M; m++)
            {
                int N = ptrndata.docs[m].length;
                z[m] = new int[N];

                // initialize for z
                for (n = 0; n < N; n++)
                {
                    int topic = random.Next(K);
//                    int topic = (int)(((double)random() / RAND_MAX) * K);
                    z[m][n] = topic;

                    // number of instances of word i assigned to topic j
                    nw[ptrndata.docs[m].words[n]][topic] += 1;
                    // number of words in document i assigned to topic j
                    nd[m][topic] += 1;
                    // total number of words assigned to topic j
                    nwsum[topic] += 1;
                }
                // total number of words in document i
                ndsum[m] = N;
            }

            theta = new double[M][];
            for (m = 0; m < M; m++)
            {
                theta[m] = new double[K];
            }

            phi = new double[K][];
            for (k = 0; k < K; k++)
            {
                phi[k] = new double[V];
            }

            return true;

        }

        bool init_estc()
        {
            // estimating the model from a previously estimated one
            int m, n, w, k;

            p = new double[K];

            // load moel, i.e., read z and ptrndata
//            if (load_model(model_name))
//            {
//                printf("Fail to load word-topic assignmetn file of the model!\n");
//                return 1;
//            }

            nw = new int[V][];
            for (w = 0; w < V; w++)
            {
                nw[w] = new int[K];
                for (k = 0; k < K; k++)
                {
                    nw[w][k] = 0;
                }
            }

            nd = new int[M][];
            for (m = 0; m < M; m++)
            {
                nd[m] = new int[K];
                for (k = 0; k < K; k++)
                {
                    nd[m][k] = 0;
                }
            }

            nwsum = new int[K];
            for (k = 0; k < K; k++)
            {
                nwsum[k] = 0;
            }

            ndsum = new int[M];
            for (m = 0; m < M; m++)
            {
                ndsum[m] = 0;
            }

            for (m = 0; m < ptrndata.M; m++)
            {
                int N = ptrndata.docs[m].length;

                // assign values for nw, nd, nwsum, and ndsum	
                for (n = 0; n < N; n++)
                {
                    int ww = ptrndata.docs[m].words[n];
                    int topic = z[m][n];

                    // number of instances of word i assigned to topic j
                    nw[ww][topic] += 1;
                    // number of words in document i assigned to topic j
                    nd[m][topic] += 1;
                    // total number of words assigned to topic j
                    nwsum[topic] += 1;
                }
                // total number of words in document i
                ndsum[m] = N;
            }

            theta = new double[M][];
            for (m = 0; m < M; m++)
            {
                theta[m] = new double[K];
            }

            phi = new double[K][];
            for (k = 0; k < K; k++)
            {
                phi[k] = new double[V];
            }

            return true;
        }


        // estimate LDA model using Gibbs sampling
        void estimate()
        {
            if (twords > 0)
            {
                // print out top words per topic
                dataset.read_wordmap(dir + wordmapfile, id2word);
            }

//            printf("Sampling %d iterations!\n", niters);

            int last_iter = liter;
            for (liter = last_iter + 1; liter <= niters + last_iter; liter++)
            {
//                printf("Iteration %d ...\n", liter);

                // for all z_i
                for (int m = 0; m < M; m++)
                {
                    for (int n = 0; n < ptrndata.docs[m].length; n++)
                    {
                        // (z_i = z[m][n])
                        // sample from p(z_i|z_-i, w)
                        int topic = sampling(m, n);
                        z[m][n] = topic;
                    }
                }

                if (savestep > 0)
                {
                    if (liter % savestep == 0)
                    {
                        // saving the model
//                        printf("Saving the model at iteration %d ...\n", liter);
                        compute_theta();
                        compute_phi();
//                        save_model(utils.generate_model_name(liter));
                    }
                }
            }

//            printf("Gibbs sampling completed!\n");
//            printf("Saving the final model!\n");
            compute_theta();
            compute_phi();
            liter--;
//            save_model(utils.generate_model_name(-1));
        }

        int sampling(int m, int n)
        {
            // remove z_i from the count variables
            int topic = z[m][n];
            int w = ptrndata.docs[m].words[n];
            nw[w][topic] -= 1;
            nd[m][topic] -= 1;
            nwsum[topic] -= 1;
            ndsum[m] -= 1;

            double Vbeta = V * beta;
            double Kalpha = K * alpha;
            // do multinomial sampling via cumulative method
            for (int k = 0; k < K; k++)
            {
                p[k] = (nw[w][k] + beta) / (nwsum[k] + Vbeta) *
                        (nd[m][k] + alpha) / (ndsum[m] + Kalpha);
            }
            // cumulate multinomial parameters
            for (int k = 1; k < K; k++)
            {
                p[k] += p[k - 1];
            }

            var random = new Random(); // todo make it field

            // scaled sample because of unnormalized p[]
            double u = random.NextDouble() * p[K - 1];
            //            double u = ((double)random() / RAND_MAX) * p[K - 1];

            for (topic = 0; topic < K; topic++)
            {
                if (p[topic] > u)
                {
                    break;
                }
            }

            // add newly estimated z_i to count variables
            nw[w][topic] += 1;
            nd[m][topic] += 1;
            nwsum[topic] += 1;
            ndsum[m] += 1;

            return topic;
        }

        void compute_theta()
        {
            for (int m = 0; m < M; m++)
            {
                for (int k = 0; k < K; k++)
                {
                    theta[m][k] = (nd[m][k] + alpha) / (ndsum[m] + K * alpha);
                }
            }
        }

        void compute_phi()
        {
            for (int k = 0; k < K; k++)
            {
                for (int w = 0; w < V; w++)
                {
                    phi[k][w] = (nw[w][k] + beta) / (nwsum[k] + V * beta);
                }
            }
        }


        // init for inference
        bool init_inf()
        {
            // estimating the model from a previously estimated one
            int m, n, w, k;

            p = new double[K];

            // load moel, i.e., read z and ptrndata
            //            if (load_model(model_name))
            //            {
            //                printf("Fail to load word-topic assignmetn file of the model!\n");
            //                return 1;
            //            }

            nw = new int[V][];
            for (w = 0; w < V; w++)
            {
                nw[w] = new int[K];
                for (k = 0; k < K; k++)
                {
                    nw[w][k] = 0;
                }
            }

            nd = new int[M][];
            for (m = 0; m < M; m++)
            {
                nd[m] = new int[K];
                for (k = 0; k < K; k++)
                {
                    nd[m][k] = 0;
                }
            }

            nwsum = new int[K];
            for (k = 0; k < K; k++)
            {
                nwsum[k] = 0;
            }

            ndsum = new int[M];
            for (m = 0; m < M; m++)
            {
                ndsum[m] = 0;
            }

            for (m = 0; m < ptrndata.M; m++)
            {
                int N = ptrndata.docs[m].length;

                // assign values for nw, nd, nwsum, and ndsum	
                for (n = 0; n < N; n++)
                {
                    int ww = ptrndata.docs[m].words[n];
                    int topic = z[m][n];

                    // number of instances of word i assigned to topic j
                    nw[ww][topic] += 1;
                    // number of words in document i assigned to topic j
                    nd[m][topic] += 1;
                    // total number of words assigned to topic j
                    nwsum[topic] += 1;
                }
                // total number of words in document i
                ndsum[m] = N;
            }

            // read new data for inference
            pnewdata = new dataset();
            if (withrawstrs)
            {
                if (pnewdata.read_newdata_withrawstrs(dir + dfile, dir + wordmapfile))
                {
                    //                    printf("Fail to read new data!\n");
                    return false;
                }
            }
            else
            {
                if (pnewdata.read_newdata(dir + dfile, dir + wordmapfile))
                {
                    //                    printf("Fail to read new data!\n");
                    return false;
                }
            }

            newM = pnewdata.M;
            newV = pnewdata.V;

            newnw = new int[newV][];
            for (w = 0; w < newV; w++)
            {
                newnw[w] = new int[K];
                for (k = 0; k < K; k++)
                {
                    newnw[w][k] = 0;
                }
            }

            newnd = new int[newM][];
            for (m = 0; m < newM; m++)
            {
                newnd[m] = new int[K];
                for (k = 0; k < K; k++)
                {
                    newnd[m][k] = 0;
                }
            }

            newnwsum = new int[K];
            for (k = 0; k < K; k++)
            {
                newnwsum[k] = 0;
            }

            newndsum = new int[newM];
            for (m = 0; m < newM; m++)
            {
                newndsum[m] = 0;
            }

            var random = new Random();
            //            srandom(time(0)); // initialize for random number generation
            newz = new int[newM][];
            for (m = 0; m < pnewdata.M; m++)
            {
                int N = pnewdata.docs[m].length;
                newz[m] = new int[N];

                // assign values for nw, nd, nwsum, and ndsum	
                for (n = 0; n < N; n++)
                {
                    int w = pnewdata.docs[m].words[n];
                    int _w = pnewdata._docs[m].words[n];
                    int topic = random.Next(K);
                    //                    int topic = (int)(((double)random() / RAND_MAX) * K);
                    newz[m][n] = topic;

                    // number of instances of word i assigned to topic j
                    newnw[_w][topic] += 1;
                    // number of words in document i assigned to topic j
                    newnd[m][topic] += 1;
                    // total number of words assigned to topic j
                    newnwsum[topic] += 1;
                }
                // total number of words in document i
                newndsum[m] = N;
            }

            newtheta = new double[newM][];
            for (m = 0; m < newM; m++)
            {
                newtheta[m] = new double[K];
            }

            newphi = new double[K][];
            for (k = 0; k < K; k++)
            {
                newphi[k] = new double[newV];
            }

            return true;
        }

        // inference for new (unseen) data based on the estimated LDA model
        void inference()
        {
            if (twords > 0)
            {
                // print out top words per topic
                dataset.read_wordmap(dir + wordmapfile, id2word);
            }

//            printf("Sampling %d iterations for inference!\n", niters);

            for (inf_liter = 1; inf_liter <= niters; inf_liter++)
            {
//                printf("Iteration %d ...\n", inf_liter);

                // for all newz_i
                for (int m = 0; m < newM; m++)
                {
                    for (int n = 0; n < pnewdata.docs[m].length; n++)
                    {
                        // (newz_i = newz[m][n])
                        // sample from p(z_i|z_-i, w)
                        int topic = inf_sampling(m, n);
                        newz[m][n] = topic;
                    }
                }
            }

//            printf("Gibbs sampling for inference completed!\n");
//            printf("Saving the inference outputs!\n");
            compute_newtheta();
            compute_newphi();
            inf_liter--;
//            save_inf_model(dfile);
        }

        int inf_sampling(int m, int n)
        {
            // remove z_i from the count variables
            int topic = newz[m][n];
            int w = pnewdata.docs[m].words[n];
            int _w = pnewdata._docs[m].words[n];
            newnw[_w][topic] -= 1;
            newnd[m][topic] -= 1;
            newnwsum[topic] -= 1;
            newndsum[m] -= 1;

            double Vbeta = V * beta;
            double Kalpha = K * alpha;
            // do multinomial sampling via cumulative method
            for (int k = 0; k < K; k++)
            {
                p[k] = (nw[w][k] + newnw[_w][k] + beta) / (nwsum[k] + newnwsum[k] + Vbeta) *
                        (newnd[m][k] + alpha) / (newndsum[m] + Kalpha);
            }
            // cumulate multinomial parameters
            for (int k = 1; k < K; k++)
            {
                p[k] += p[k - 1];
            }
            // scaled sample because of unnormalized p[]
            double u = new Random().NextDouble() * p[K - 1]; // todo random
                                                             //            double u = ((double)random() / RAND_MAX) * p[K - 1];

            for (topic = 0; topic < K; topic++)
            {
                if (p[topic] > u)
                {
                    break;
                }
            }

            // add newly estimated z_i to count variables
            newnw[_w][topic] += 1;
            newnd[m][topic] += 1;
            newnwsum[topic] += 1;
            newndsum[m] += 1;

            return topic;
        }

        void compute_newtheta()
        {
            for (int m = 0; m < newM; m++)
            {
                for (int k = 0; k < K; k++)
                {
                    newtheta[m][k] = (newnd[m][k] + alpha) / (newndsum[m] + K * alpha);
                }
            }
        }

        void compute_newphi()
        {
            Dictionary<int, int> it;
            for (int k = 0; k < K; k++)
            {
                for (int w = 0; w < newV; w++)
                {
                    int ww;
                    if (pnewdata._id2id.TryGetValue(w, out ww))
                    {
                        newphi[k][w] = (nw[ww][k] + newnw[w][k] + beta) / (nwsum[k] + newnwsum[k] + V * beta);
                    }
//                    it = pnewdata->_id2id.find(w);
//                    if (it != pnewdata->_id2id.end())
//                    {
//                        newphi[k][w] = (nw[it->second][k] + newnw[w][k] + beta) / (nwsum[k] + newnwsum[k] + V * beta);
//                    }
                }
            }
        }

    };


}