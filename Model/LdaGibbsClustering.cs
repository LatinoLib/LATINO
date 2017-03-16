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

    public class Document
    {
        public int Length { get; private set; }
        public string Rawstr { get; private set; }
        public int[] Words { get; private set; }

        public Document()
        {
            Words = null;
            Rawstr = "";
            Length = 0;
        }

        public Document(int length)
        {
            this.Length = length;
            Rawstr = "";
            Words = new int[length];
        }

        public Document(int length, int[] words)
        {
            this.Length = length;
            Rawstr = "";
            this.Words = new int[length];
            for (var i = 0; i < length; i++)
                this.Words[i] = words[i];
        }

        public Document(int length, int[] words, string rawstr)
        {
            this.Length = length;
            this.Rawstr = rawstr;
            this.Words = new int[length];
            for (var i = 0; i < length; i++)
                this.Words[i] = words[i];
        }
    }

    public class Dataset
    {
        public Document[] _docs; // used only for inference
        public Dictionary<int, int> _id2id; // also used only for inference
        public Document[] docs;
        public int M; // number of documents
        public int V; // number of words

        public Dataset()
        {
            docs = null;
            _docs = null;
            M = 0;
            V = 0;
        }

        public Dataset(int M)
        {
            this.M = M;
            V = 0;
            docs = new Document[M];
            _docs = null;
        }

        public void add_doc(Document doc, int idx)
        {
            if (0 <= idx && idx < M)
                docs[idx] = doc;
        }

        public void _add_doc(Document doc, int idx)
        {
            if (0 <= idx && idx < M)
                _docs[idx] = doc;
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
        private const int MODEL_STATUS_UNKNOWN = 0;
        private const int MODEL_STATUS_EST = 1;
        private const int MODEL_STATUS_ESTC = 2;
        private const int MODEL_STATUS_INF = 3;


        private double alpha, beta; // LDA hyperparameters 
        private string dfile; // data file    

        private string dir; // model directory

        private Dictionary<int, string> id2word; // word map [int => string]

        // for inference only
        private int inf_liter;
        private int K; // number of topics
        private int liter; // the iteration at which the model was saved

        // --- model parameters and variables ---    
        private int M; // dataset size (i.e., number of docs)
        private string model_name; // model name
        private int model_status; // model status:
        private int[][] nd; // na[i][j]: number of words in document i assigned to topic j, size M x K
        private int[] ndsum; // nasum[i]: total number of words in document i, size M
        private int newM;
        private int[][] newnd;
        private int[] newndsum;
        private int[][] newnw;
        private int[] newnwsum;
        private double[][] newphi;
        private double[][] newtheta;
        private int newV;
        private int[][] newz;
        private int niters; // number of Gibbs sampling iterations
        private int[][] nw; // cwt[i][j]: number of instances of word/term i assigned to topic j, size V x K
        private int[] nwsum; // nwsum[j]: total number of words assigned to topic j, size K
        private string others_suffix; // suffix for file containing other parameters

        private double[] p; // temp variable for sampling
        public double[][] phi; // phi: topic-word distributions, size K x V
        private string phi_suffix; // suffix for phi file
        private Dataset newdata; // pointer to new dataset object
        // MODEL_STATUS_UNKNOWN: unknown status
        // MODEL_STATUS_EST: estimating from scratch
        // MODEL_STATUS_ESTC: continue to estimate the model from a previous one
        // MODEL_STATUS_INF: do inference

        public Dataset ndata; // pointer to training dataset object
        private int savestep; // saving period
        private string tassign_suffix; // suffix for topic assignment file
        public double[][] theta; // theta: document-topic distributions, size M x K
        private string theta_suffix; // suffix for theta file
        private string trainlogfile; // training log file
        private int twords; // print out top words per each topic
        private string twords_suffix; // suffix for file containing words-per-topics
        private int V; // vocabulary size
        private bool withrawstrs;

        private Random mRandom;

        // fixed options
        private string wordmapfile; // file that contains word map [string . integer id]
        private int[][] z; // topic assignments for words, size M x doc.size()
        // --------------------------------------

        public model()
        {
            set_default_values();
        }


        // set default values for variables
        private void set_default_values()
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

            ndata = null;
            newdata = null;

            M = 0;
            V = 0;
            K = 100;
            alpha = 50.0 / K;
            beta = 0.1;
            niters = 20;
            liter = 0;
            savestep = 200;
            twords = 0;
            withrawstrs = false;

//            p = null;
//            z = null;
//            nw = null;
//            nd = null;
//            nwsum = null;
//            ndsum = null;
            //            theta = null;
            //            phi = null;

//            newM = 0;
//            newV = 0;
            //            newz = null;
            //            newnw = null;
            //            newnd = null;
            //            newnwsum = null;
            //            newndsum = null;
            //            newtheta = null;
            //            newphi = null;
        }


        // initialize the model
        private bool init(int argc, char argv)
        {
            if (model_status == MODEL_STATUS_EST)
            {
                // estimating the model from scratch
                if (init_est())
                    return false;
            }
            else if (model_status == MODEL_STATUS_ESTC)
            {
                // estimating the model from a previously estimated one
                if (init_estc())
                    return false;
            }
            else if (model_status == MODEL_STATUS_INF)
            {
                // do inference
                if (init_inf())
                    return false;
            }

            return true;
        }


        // init for estimation
        public bool init_est()
        {
            int m, n, w, k;

            p = new double[K];

            // + read training data
//            ndata = new Dataset();
            //            if (ptrndata.read_trndata(dir + dfile, dir + wordmapfile))
            //            {
            //                printf("Fail to read training data!\n");
            //                return 1;
            //            }

            // + allocate memory and assign values for variables
            M = ndata.M;
            V = ndata.V;
            // K: from command line or default value
            // alpha, beta: from command line or default values
            // niters, savestep: from command line or default values

            nw = new int[V][];
            for (w = 0; w < V; w++)
            {
                nw[w] = new int[K];
                for (k = 0; k < K; k++)
                    nw[w][k] = 0;
            }

            nd = new int[M][];
            for (m = 0; m < M; m++)
            {
                nd[m] = new int[K];
                for (k = 0; k < K; k++)
                    nd[m][k] = 0;
            }

            nwsum = new int[K];
            for (k = 0; k < K; k++)
                nwsum[k] = 0;

            ndsum = new int[M];
            for (m = 0; m < M; m++)
                ndsum[m] = 0;

            mRandom = new Random(); // todo random 
            //            srandom(time(0)); // initialize for random number generation
            z = new int[M][];
            for (m = 0; m < ndata.M; m++)
            {
                int N = ndata.docs[m].Length;
                z[m] = new int[N];

                // initialize for z
                for (n = 0; n < N; n++)
                {
                    int topic = mRandom.Next(K);
                    //                    int topic = (int)(((double)random() / RAND_MAX) * K);
                    z[m][n] = topic;

                    // number of instances of word i assigned to topic j
                    nw[ndata.docs[m].Words[n]][topic] += 1;
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
                theta[m] = new double[K];

            phi = new double[K][];
            for (k = 0; k < K; k++)
                phi[k] = new double[V];

            return true;
        }

        public bool init_estc()
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
                    nw[w][k] = 0;
            }

            nd = new int[M][];
            for (m = 0; m < M; m++)
            {
                nd[m] = new int[K];
                for (k = 0; k < K; k++)
                    nd[m][k] = 0;
            }

            nwsum = new int[K];
            for (k = 0; k < K; k++)
                nwsum[k] = 0;

            ndsum = new int[M];
            for (m = 0; m < M; m++)
                ndsum[m] = 0;

            for (m = 0; m < ndata.M; m++)
            {
                int N = ndata.docs[m].Length;

                // assign values for nw, nd, nwsum, and ndsum	
                for (n = 0; n < N; n++)
                {
                    int ww = ndata.docs[m].Words[n];
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
                theta[m] = new double[K];

            phi = new double[K][];
            for (k = 0; k < K; k++)
                phi[k] = new double[V];

            return true;
        }


        // estimate LDA model using Gibbs sampling
        public void estimate()
        {
            if (twords > 0)
                Dataset.read_wordmap(dir + wordmapfile, id2word);

            //            printf("Sampling %d iterations!\n", niters);

            int last_iter = liter;
            for (liter = last_iter + 1; liter <= niters + last_iter; liter++)
            {
                //                printf("Iteration %d ...\n", liter);

                // for all z_i
                for (var m = 0; m < M; m++)
                for (var n = 0; n < ndata.docs[m].Length; n++)
                {
                    // (z_i = z[m][n])
                    // sample from p(z_i|z_-i, w)
                    int topic = sampling(m, n);
                    z[m][n] = topic;
                }

                if (savestep > 0)
                    if (liter % savestep == 0)
                    {
                        // saving the model
                        //                        printf("Saving the model at iteration %d ...\n", liter);
                        compute_theta();
                        compute_phi();
                        //                        save_model(utils.generate_model_name(liter));
                    }
            }

            //            printf("Gibbs sampling completed!\n");
            //            printf("Saving the final model!\n");
            compute_theta();
            compute_phi();
            liter--;
            //            save_model(utils.generate_model_name(-1));
        }

        private int sampling(int m, int n)
        {
            // remove z_i from the count variables
            int topic = z[m][n];
            int w = ndata.docs[m].Words[n];
            nw[w][topic] -= 1;
            nd[m][topic] -= 1;
            nwsum[topic] -= 1;
            ndsum[m] -= 1;

            double Vbeta = V * beta;
            double Kalpha = K * alpha;
            // do multinomial sampling via cumulative method
            for (var k = 0; k < K; k++)
                p[k] = (nw[w][k] + beta) / (nwsum[k] + Vbeta) *
                       (nd[m][k] + alpha) / (ndsum[m] + Kalpha);
            // cumulate multinomial parameters
            for (var k = 1; k < K; k++)
                p[k] += p[k - 1];

            // scaled sample because of unnormalized p[]
            double u = mRandom.NextDouble() * p[K - 1];
            //            double u = ((double)random() / RAND_MAX) * p[K - 1];

            for (topic = 0; topic < K; topic++)
                if (p[topic] > u)
                    break;

            // add newly estimated z_i to count variables
            nw[w][topic] += 1;
            nd[m][topic] += 1;
            nwsum[topic] += 1;
            ndsum[m] += 1;

            return topic;
        }

        private void compute_theta()
        {
            for (var m = 0; m < M; m++)
            for (var k = 0; k < K; k++)
                theta[m][k] = (nd[m][k] + alpha) / (ndsum[m] + K * alpha);
        }

        private void compute_phi()
        {
            for (var k = 0; k < K; k++)
            for (var w = 0; w < V; w++)
                phi[k][w] = (nw[w][k] + beta) / (nwsum[k] + V * beta);
        }


        // init for inference
        public bool init_inf()
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
                    nw[w][k] = 0;
            }

            nd = new int[M][];
            for (m = 0; m < M; m++)
            {
                nd[m] = new int[K];
                for (k = 0; k < K; k++)
                    nd[m][k] = 0;
            }

            nwsum = new int[K];
            for (k = 0; k < K; k++)
                nwsum[k] = 0;

            ndsum = new int[M];
            for (m = 0; m < M; m++)
                ndsum[m] = 0;

            for (m = 0; m < ndata.M; m++)
            {
                int N = ndata.docs[m].Length;

                // assign values for nw, nd, nwsum, and ndsum	
                for (n = 0; n < N; n++)
                {
                    int ww = ndata.docs[m].Words[n];
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
//            newdata = new Dataset();
//            if (withrawstrs)
//            {
//                if (newdata.read_newdata_withrawstrs(dir + dfile, dir + wordmapfile))
//                    return false;
//            }
//            else
//            {
//                if (newdata.read_newdata(dir + dfile, dir + wordmapfile))
//                    return false;
//            }

            newM = newdata.M;
            newV = newdata.V;

            newnw = new int[newV][];
            for (w = 0; w < newV; w++)
            {
                newnw[w] = new int[K];
                for (k = 0; k < K; k++)
                    newnw[w][k] = 0;
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

            mRandom = new Random();
            //            srandom(time(0)); // initialize for random number generation
            newz = new int[newM][];
            for (m = 0; m < newdata.M; m++)
            {
                int N = newdata.docs[m].Length;
                newz[m] = new int[N];

                // assign values for nw, nd, nwsum, and ndsum	
                for (n = 0; n < N; n++)
                {
                    int ww = newdata.docs[m].Words[n];
                    int _w = newdata._docs[m].Words[n];
                    int topic = mRandom.Next(K);
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
                newtheta[m] = new double[K];

            newphi = new double[K][];
            for (k = 0; k < K; k++)
                newphi[k] = new double[newV];

            return true;
        }

        // inference for new (unseen) data based on the estimated LDA model
        private void inference()
        {
            if (twords > 0)
                Dataset.read_wordmap(dir + wordmapfile, id2word);

            //            printf("Sampling %d iterations for inference!\n", niters);

            for (inf_liter = 1; inf_liter <= niters; inf_liter++)
            for (var m = 0; m < newM; m++)
            for (var n = 0; n < newdata.docs[m].Length; n++)
            {
                // (newz_i = newz[m][n])
                // sample from p(z_i|z_-i, w)
                int topic = inf_sampling(m, n);
                newz[m][n] = topic;
            }

            //            printf("Gibbs sampling for inference completed!\n");
            //            printf("Saving the inference outputs!\n");
            compute_newtheta();
            compute_newphi();
            inf_liter--;
            //            save_inf_model(dfile);
        }

        private int inf_sampling(int m, int n)
        {
            // remove z_i from the count variables
            int topic = newz[m][n];
            int w = newdata.docs[m].Words[n];
            int _w = newdata._docs[m].Words[n];
            newnw[_w][topic] -= 1;
            newnd[m][topic] -= 1;
            newnwsum[topic] -= 1;
            newndsum[m] -= 1;

            double Vbeta = V * beta;
            double Kalpha = K * alpha;
            // do multinomial sampling via cumulative method
            for (var k = 0; k < K; k++)
                p[k] = (nw[w][k] + newnw[_w][k] + beta) / (nwsum[k] + newnwsum[k] + Vbeta) *
                       (newnd[m][k] + alpha) / (newndsum[m] + Kalpha);
            // cumulate multinomial parameters
            for (var k = 1; k < K; k++)
                p[k] += p[k - 1];
            // scaled sample because of unnormalized p[]
            double u = mRandom.NextDouble() * p[K - 1]; // todo random
            //            double u = ((double)random() / RAND_MAX) * p[K - 1];

            for (topic = 0; topic < K; topic++)
                if (p[topic] > u)
                    break;

            // add newly estimated z_i to count variables
            newnw[_w][topic] += 1;
            newnd[m][topic] += 1;
            newnwsum[topic] += 1;
            newndsum[m] += 1;

            return topic;
        }

        private void compute_newtheta()
        {
            for (var m = 0; m < newM; m++)
            for (var k = 0; k < K; k++)
                newtheta[m][k] = (newnd[m][k] + alpha) / (newndsum[m] + K * alpha);
        }

        private void compute_newphi()
        {
            for (var k = 0; k < K; k++)
            for (var w = 0; w < newV; w++)
            {
                int ww;
                if (newdata._id2id.TryGetValue(w, out ww))
                    newphi[k][w] = (nw[ww][k] + newnw[w][k] + beta) / (nwsum[k] + newnwsum[k] + V * beta);
                //                    it = pnewdata->_id2id.find(w);
                //                    if (it != pnewdata->_id2id.end())
                //                    {
                //                        newphi[k][w] = (nw[it->second][k] + newnw[w][k] + beta) / (nwsum[k] + newnwsum[k] + V * beta);
                //                    }
            }
        }
    }
}