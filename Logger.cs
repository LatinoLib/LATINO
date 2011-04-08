/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Logger.cs
 *  Desc:    Logging utility
 *  Created: Dec-2010
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Logger
       |
       '-----------------------------------------------------------------------
    */
    public class Logger
    {
        /* .-----------------------------------------------------------------------
           |
           |  Class Node
           |
           '-----------------------------------------------------------------------
        */
        private class Node
        {          
            public Logger mLogger
                = null;
            public Dictionary<string, Node> mChildren
                = new Dictionary<string, Node>();

            private void PropagateSettings(Level level, OutputType outType, TextWriter writer)
            {
                foreach (KeyValuePair<string, Node> item in mChildren)
                {
                    if (item.Value.mLogger != null)
                    {
                        item.Value.mLogger.Inherit(level, outType, writer);
                        item.Value.PropagateSettings();
                    }
                    else
                    {
                        item.Value.PropagateSettings(level, outType, writer);
                    }
                }
            }

            public void PropagateSettings()
            {
                PropagateSettings(mLogger.ActiveLevel, mLogger.ActiveOutputType, mLogger.ActiveWriter);
            }

            //public void DebugOut(string prefix)
            //{
            //    Console.Write(prefix);
            //    Console.Write(mLogger == null ? "null" : (mLogger.Name == null ? "root" : mLogger.Name));
            //    if (mLogger == null) { Console.WriteLine(); }
            //    else { Console.WriteLine(" ({0}; {1})", mLogger.ActiveLevel, mLogger.ActiveOutputType); }
            //    foreach (KeyValuePair<string, Node> item in mChildren)
            //    {
            //        item.Value.DebugOut(prefix + "\t");
            //    }
            //}
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum OutputType
           |
           '-----------------------------------------------------------------------
        */
        [Flags]
        public enum OutputType
        { 
            Inherit = 0,
            Console = 1,                       
            Writer  = 2,
            //Custom  = 4
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum ProgressOutputType
           |
           '-----------------------------------------------------------------------
        */
        public enum ProgressOutputType
        {
            Inherit = 0,
            Console = 1,
            //Custom = 2,
            Off = 3
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum Level
           |
           '-----------------------------------------------------------------------
        */
        public enum Level
        { 
            Inherit = 0,
            Trace   = 1,
            Debug   = 2,
            Info    = 3,
            Warn    = 4,
            Error   = 5,
            Fatal   = 6,
            Off     = 7
        }

        private static Node mRoot;

        private static object mConsoleLock
            = new object();
        private static object mProgressSender
            = null;

        // node settings         
        private string mName; // set by constructor
        private Level mLocalLevel
            = Level.Inherit;
        private OutputType mLocalOutputType
            = OutputType.Inherit;
        private TextWriter mLocalWriter
            = null;

        // inherited settings (set by constructor)
        private Level mInheritedLevel;
        private OutputType mInheritedOutputType;
        private TextWriter mInheritedWriter;

        private Node mNode;

        static Logger()
        {
            Logger logger = new Logger(/*name=*/null, Level.Info, OutputType.Console, /*writer=*/null, mRoot = new Node());
            mRoot.mLogger = logger;
            mRoot.mLogger.mLocalLevel = Level.Info;
            mRoot.mLogger.mLocalOutputType = OutputType.Console;
        }

        private Logger(string name, Level level, OutputType outType, TextWriter writer, Node node)
        {
            mName = name;
            mInheritedLevel = level;
            mInheritedOutputType = outType;
            mInheritedWriter = writer;
            mNode = node;
        }

        public static Logger GetRootLogger()
        {
            return mRoot.mLogger;
        }

        public static Logger GetLogger(Type type)
        {
            if (type == null) { return mRoot.mLogger; }
            return GetLogger(type.ToString());
        }

        public static Logger GetLogger(string name)
        {
            if (name == null) { return mRoot.mLogger; }
            lock (mRoot)
            {                
                string[] nodes = name.Split('.');
                Node node = mRoot;
                Level level = node.mLogger.mLocalLevel;
                OutputType outType = node.mLogger.mLocalOutputType;
                TextWriter writer = node.mLogger.mLocalWriter;
                for (int i = 0; i < nodes.Length; i++)
                {
                    string nodeName = nodes[i];
                    if (node.mChildren.ContainsKey(nodeName))
                    {
                        node = node.mChildren[nodeName];
                        if (node.mLogger != null)
                        {
                            level = node.mLogger.ActiveLevel;
                            outType = node.mLogger.ActiveOutputType;
                            writer = node.mLogger.ActiveWriter;
                        }
                        else if (i == nodes.Length - 1)
                        {
                            node.mLogger = new Logger(name, level, outType, writer, node);
                        }
                    }
                    else if (i == nodes.Length - 1)
                    {
                        Node newNode = new Node();
                        newNode.mLogger = new Logger(name, level, outType, writer, newNode);
                        node.mChildren.Add(nodeName, node = newNode);
                    }
                    else
                    {
                        node.mChildren.Add(nodeName, node = new Node());
                    }
                }
                return node.mLogger;
            }
        }

        public string Name
        {
            get { return mName; }
        }

        internal void Inherit(Level level, OutputType outType, TextWriter writer)
        {
            mInheritedLevel = level;
            mInheritedOutputType = outType;
            mInheritedWriter = writer;
        }

        public Level LocalLevel
        {
            get { return mLocalLevel; }
            set 
            { 
                lock (mRoot)
                {
                    mLocalLevel = value;
                    mNode.PropagateSettings();
                }
            }
        }

        public OutputType LocalOutputType
        {
            get { return mLocalOutputType; }
            set 
            {
                lock (mRoot)
                {
                    mLocalOutputType = value;
                    mNode.PropagateSettings();
                }
            }
        }

        public TextWriter LocalWriter
        {
            get { return mLocalWriter; }
            set
            {
                lock (mRoot)
                {
                    mLocalWriter = value;
                    mNode.PropagateSettings();
                }
            }
        }

        public Level ActiveLevel
        {
            get { return mLocalLevel == Level.Inherit ? mInheritedLevel : mLocalLevel; }
        }

        public OutputType ActiveOutputType
        {
            get { return mLocalOutputType == OutputType.Inherit ? mInheritedOutputType : mLocalOutputType; }
        }

        public TextWriter ActiveWriter
        {
            get { return mLocalOutputType == OutputType.Inherit ? mInheritedWriter : mLocalWriter; }
        }

        //public void DebugOut()
        //{
        //    mNode.DebugOut("");
        //}

        private void Output(TextWriter writer, Level level, string funcName, Exception e, string message, params object[] args)
        {
            string levelStr = level.ToString().ToUpper();
            if (funcName == null) { funcName = "(null)"; }
            if (message == null)
            {
                if (e != null && e.Message != null) { message = e.Message; }
                else { message = "(null)"; }
            }
            writer.WriteLine("{0:yyyy-MM-dd HH:mm:ss} {1} {2}", DateTime.Now, mName == null ? "(root)" : mName, funcName);
            if (args.Length > 0) { writer.WriteLine("{0}: {1}", levelStr, string.Format(message, args)); }
            else { writer.WriteLine("{0}: {1}", levelStr, message); }
            if (e != null && e.StackTrace != null) { writer.WriteLine(e.StackTrace); }
            writer.Flush();
        }

        private void Output(Level level, string funcName, Exception e, string message, params object[] args)
        {
            if (ActiveLevel <= level)
            {
                OutputType activeOutType = ActiveOutputType;
                TextWriter activeWriter = ActiveWriter;
                if ((activeOutType & OutputType.Console) != 0)
                {
                    lock (mConsoleLock)
                    {
                        if (mProgressSender != null) { mProgressSender = null; Console.WriteLine(); }
                        Output(Console.Out, level, funcName, e, message, args);
                    }
                }
                if ((activeOutType & OutputType.Writer) != 0 && activeWriter != null)
                {
                    lock (activeWriter)
                    {
                        Output(activeWriter, level, funcName, e, message, args);
                    }
                }
            }
        }

        private void Progress(object sender, int freq, string funcName, string message, int step, int numSteps, params object[] args)
        {
            //if (ActiveOutputProgress)
            {
                if (numSteps <= 0)
                {
                    if (step % freq == 0)
                    {
                        lock (mConsoleLock)
                        {
                            if (mProgressSender != null /*&& mProgressSender != sender*/) { mProgressSender = sender; Console.WriteLine(); }
                            Console.Write("\r" + message, step); // throws FormatException
                        }
                    }
                }
                else
                {
                    if (step % freq == 0 || step == numSteps)
                    {
                        lock (mConsoleLock)
                        {
                            if (mProgressSender != null /*&& mProgressSender != sender*/) { mProgressSender = sender; Console.WriteLine(); }
                            Console.Write("\r" + message, step, numSteps); // throws FormatException
                            if (step == numSteps) { Console.WriteLine(); }
                        }
                    }
                }
            }
        }

        // *** Public interface ***

        public void Trace(string funcName, string message, params object[] args)
        {
            Output(Level.Trace, funcName, /*e=*/null, message, args); // throws FormatException
        }

        public void Debug(string funcName, string message, params object[] args)
        {
            Output(Level.Debug, funcName, /*e=*/null, message, args); // throws FormatException     
        }

        public void Info(string funcName, string message, params object[] args)
        {
            Output(Level.Info, funcName, /*e=*/null, message, args); // throws FormatException
        }

        public void Warn(string funcName, string message, params object[] args)
        {
            Output(Level.Warn, funcName, /*e=*/null, message, args); // throws FormatException
        }

        public void Warn(string funcName, Exception e)
        {
            Output(Level.Warn, funcName, e, /*message=*/null);
        }

        public void Error(string funcName, string message, params object[] args)
        {
            Output(Level.Error, funcName, /*e=*/null, message, args); // throws FormatException
        }

        public void Error(string funcName, Exception e)
        {
            Output(Level.Error, funcName, e, /*message=*/null);
        }

        public void Fatal(string funcName, string message, params object[] args)
        {
            Output(Level.Fatal, funcName, /*e=*/null, message, args); // throws FormatException
        }

        public void Fatal(string funcName, Exception e)
        {
            Output(Level.Fatal, funcName, e, /*message=*/null);
        }

        public void ProgressNormal(object sender, string funcName, string message, int step, int numSteps, params object[] args)
        {
            Utils.ThrowException(step < 0 ? new ArgumentOutOfRangeException("step") : null);
            Progress(sender, /*freq=*/1, funcName, message, step, numSteps, args); // throws FormatException
        }

        public void ProgressFast(object sender, string funcName, string message, int step, int numSteps, params object[] args)
        {
            Utils.ThrowException(step < 0 ? new ArgumentOutOfRangeException("step") : null);
            Progress(sender, /*freq=*/100, funcName, message, step, numSteps, args); // throws FormatException 
        }

        public void ProgressVeryFast(object sender, string funcName, string message, int step, int numSteps, params object[] args)
        {
            Utils.ThrowException(step < 0 ? new ArgumentOutOfRangeException("step") : null);
            Progress(sender, /*freq=*/1000, funcName, message, step, numSteps, args); // throws FormatException 
        }
    }
}
