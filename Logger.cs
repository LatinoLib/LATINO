/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Logger.cs
 *  Desc:    Logging utility
 *  Created: Dec-2010
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
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

            private void PropagateSettings(Level level, OutputType outType, ProgressOutputType progressOutType, TextWriter outputWriter)
            {
                foreach (KeyValuePair<string, Node> item in mChildren)
                {
                    if (item.Value.mLogger != null)
                    {
                        item.Value.mLogger.Inherit(level, outType, progressOutType, outputWriter);
                        item.Value.PropagateSettings();
                    }
                    else
                    {
                        item.Value.PropagateSettings(level, outType, progressOutType, outputWriter);
                    }
                }
            }

            public void PropagateSettings()
            {
                PropagateSettings(mLogger.ActiveLevel, mLogger.ActiveOutputType, mLogger.ActiveProgressOutputType, mLogger.ActiveOutputWriter);
            }
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
            Custom  = 4
        }

        /* .-----------------------------------------------------------------------
           |
           |  Enum ProgressOutputType
           |
           '-----------------------------------------------------------------------
        */
        [Flags]
        public enum ProgressOutputType
        {
            Inherit = 0,
            Console = 1,
            Custom  = 2,
            Off     = 4
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

        public delegate void CustomOutputDelegate(string loggerName, Level level, string funcName, Exception e, 
            string message, object[] args);
        public delegate void CustomProgressOutputDelegate(string loggerName, object sender, int freq, string funcName, 
            string message, int step, int numSteps, object[] args);

        private static Node mRoot;

        private static object mConsoleLock
            = new object();
        private static object mProgressSender
            = null;
        private static object mDefaultProgressSender
            = new object();

        private static CustomOutputDelegate mCustomOutput
            = null;
        private static CustomProgressOutputDelegate mCustomProgressOutput
            = null;
        private static bool mDisableCustomOutputLevelFilter
            = false;

        // node settings         
        private string mName; // set by the constructor
        private Level mLocalLevel
            = Level.Inherit;
        private OutputType mLocalOutputType
            = OutputType.Inherit;
        private ProgressOutputType mLocalProgressOutputType
            = ProgressOutputType.Inherit;
        private TextWriter mLocalOutputWriter
            = null; // inherit

        // inherited settings (set by the constructor)
        private Level mInheritedLevel;
        private OutputType mInheritedOutputType;
        private ProgressOutputType mInheritedProgressOutputType;
        private TextWriter mInheritedOutputWriter;

        private Node mNode;

        static Logger()
        {
            Logger logger = new Logger(/*name=*/null, Level.Info, OutputType.Console, ProgressOutputType.Console, /*outputWriter=*/null, mRoot = new Node());
            mRoot.mLogger = logger;
            mRoot.mLogger.mLocalLevel = Level.Info;
            mRoot.mLogger.mLocalOutputType = OutputType.Console;
            mRoot.mLogger.mLocalProgressOutputType = ProgressOutputType.Console;
        }

        private Logger(string name, Level level, OutputType outType, ProgressOutputType progressOutType, TextWriter outputWriter, Node node)
        {
            mName = name;
            mInheritedLevel = level;
            mInheritedOutputType = outType;
            mInheritedProgressOutputType = progressOutType;
            mInheritedOutputWriter = outputWriter;
            mNode = node;
        }

        public static Logger GetRootLogger()
        {
            return mRoot.mLogger;
        }

        private static Node GetNode(string name)
        {
            if (name == null) { return mRoot; }
            string[] nodes = name.Split('.');
            Node node = mRoot;
            Level level = node.mLogger.mLocalLevel;
            OutputType outType = node.mLogger.mLocalOutputType;
            ProgressOutputType progressOutType = node.mLogger.mLocalProgressOutputType;
            TextWriter outputWriter = node.mLogger.mLocalOutputWriter;
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
                        progressOutType = node.mLogger.ActiveProgressOutputType;
                        outputWriter = node.mLogger.ActiveOutputWriter;
                    }
                    else if (i == nodes.Length - 1)
                    {
                        node.mLogger = new Logger(name, level, outType, progressOutType, outputWriter, node);
                    }
                }
                else if (i == nodes.Length - 1)
                {
                    Node newNode = new Node();
                    newNode.mLogger = new Logger(name, level, outType, progressOutType, outputWriter, newNode);
                    node.mChildren.Add(nodeName, node = newNode);
                }
                else
                {
                    node.mChildren.Add(nodeName, node = new Node());
                }
            }
            return node;
        }

        public static Logger GetLogger(string name)
        {
            lock (mRoot)
            {
                return GetNode(name).mLogger;
            }
        }

        public static Logger GetLogger(Type type)
        {
            if (type == null) { return mRoot.mLogger; }
            return GetLogger(type.ToString());
        }

        public static Logger GetInstanceLogger(string className)
        {
            lock (mRoot)
            {
                Node node = GetNode(className);
                ulong maxVal = 0;
                foreach (string key in node.mChildren.Keys)
                {
                    ulong val;
                    if (ulong.TryParse(key, out val)) { if (val > maxVal) { maxVal = val; } }
                }
                return GetNode(className + "." + (maxVal + 1)).mLogger;
            }
        }

        public static Logger GetInstanceLogger(Type type)
        {
            if (type == null) { return mRoot.mLogger; }
            return GetInstanceLogger(type.ToString());        
        }

        public string Name
        {
            get { return mName; }
        }

        internal void Inherit(Level level, OutputType outType, ProgressOutputType progressOutType, TextWriter outputWriter)
        {
            mInheritedLevel = level;
            mInheritedOutputType = outType;
            mInheritedProgressOutputType = progressOutType;
            mInheritedOutputWriter = outputWriter;
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

        public ProgressOutputType LocalProgressOutputType
        {
            get { return mLocalProgressOutputType; }
            set
            {
                lock (mRoot)
                {
                    mLocalProgressOutputType = value;
                    mNode.PropagateSettings();
                }
            }
        }

        public TextWriter LocalOutputWriter
        {
            get { return mLocalOutputWriter; }
            set
            {
                lock (mRoot)
                {
                    mLocalOutputWriter = value;
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

        public ProgressOutputType ActiveProgressOutputType
        {
            get { return mLocalProgressOutputType == ProgressOutputType.Inherit ? mInheritedProgressOutputType : mLocalProgressOutputType; }
        }

        public TextWriter ActiveOutputWriter
        {
            get { return mLocalOutputWriter == null ? mInheritedOutputWriter : mLocalOutputWriter; }
        }

        public static CustomOutputDelegate CustomOutput
        {
            get { return mCustomOutput; }
            set { mCustomOutput = value; }
        }

        public static CustomProgressOutputDelegate CustomProgressOutput
        {
            get { return mCustomProgressOutput; }
            set { mCustomProgressOutput = value; }
        }

        public static bool DisableCustomOutputLevelFilter
        {
            get { return mDisableCustomOutputLevelFilter; }
            set { mDisableCustomOutputLevelFilter = value; }
        }

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
            if (args != null && args.Length > 0) { writer.WriteLine("{0}: {1}", levelStr, string.Format(message, args)); }
            else { writer.WriteLine("{0}: {1}", levelStr, message); }
            if (e != null && e.StackTrace != null) { writer.WriteLine(e.StackTrace); }
            writer.Flush();
        }

        private void Output(Level level, string funcName, Exception e, string message, params object[] args)
        {
            if (ActiveLevel <= level)
            {
                OutputType activeOutType = ActiveOutputType;
                TextWriter activeOutWriter = ActiveOutputWriter;
                if ((activeOutType & OutputType.Console) != 0)
                {
                    lock (mConsoleLock)
                    {
                        if (mProgressSender != null) { mProgressSender = null; Console.WriteLine(); }
                        Output(Console.Out, level, funcName, e, message, args);
                    }
                }
                if ((activeOutType & OutputType.Writer) != 0 && activeOutWriter != null)
                {
                    lock (activeOutWriter)
                    {
                        Output(activeOutWriter, level, funcName, e, message, args);
                    }                
                }
            }
            if ((ActiveOutputType & OutputType.Custom) != 0 && mCustomOutput != null && (mDisableCustomOutputLevelFilter || ActiveLevel <= level))
            {
                mCustomOutput(mName, level, funcName, e, message, args);
            }
        }

        private void Progress(Level level, object sender, int freq, string funcName, string message, int step, int numSteps, params object[] args)
        {
            ProgressOutputType activeProgressOutType = ActiveProgressOutputType;
            if ((activeProgressOutType & ProgressOutputType.Off) == 0 && ActiveLevel <= level)
            {
                if ((activeProgressOutType & ProgressOutputType.Console) != 0)
                {
                    if (numSteps <= 0)
                    {
                        if (step % freq == 0)
                        {
                            lock (mConsoleLock)
                            {
                                if (message == null) { message = "{0}"; }
                                if (mProgressSender != null && mProgressSender != sender) { Console.WriteLine(); }
                                if (args != null && args.Length > 0)
                                {
                                    object[] allArgs = new object[1 + args.Length];
                                    allArgs[0] = step;
                                    Array.Copy(args, 0, allArgs, 1, args.Length);
                                    Console.Write("\r" + message, allArgs);
                                }
                                else
                                {
                                    Console.Write("\r" + message, step);
                                }
                                mProgressSender = sender;
                            }
                        }  
                    }
                    else 
                    {
                        if (step % freq == 0 || step == numSteps)
                        {
                            lock (mConsoleLock)
                            {
                                if (message == null) { message = "{0} / {1}"; }
                                if (mProgressSender != null && mProgressSender != sender) { Console.WriteLine(); }
                                if (args != null && args.Length > 0)
                                {
                                    object[] allArgs = new object[2 + args.Length];
                                    allArgs[0] = step;
                                    allArgs[1] = numSteps;
                                    Array.Copy(args, 0, allArgs, 2, args.Length);
                                    Console.Write("\r" + message, allArgs);
                                }
                                else
                                {
                                    Console.Write("\r" + message, step, numSteps);
                                }
                                mProgressSender = sender;
                                if (step == numSteps) { mProgressSender = null; Console.WriteLine(); }
                            }
                        }
                    }
                }
            }
            if ((activeProgressOutType & ProgressOutputType.Custom) != 0 && mCustomProgressOutput != null && (mDisableCustomOutputLevelFilter || ((activeProgressOutType & ProgressOutputType.Off) == 0 && ActiveLevel <= level)))
            {
                mCustomProgressOutput(mName, sender, freq, funcName, message, step, numSteps, args);
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

        public void ProgressNormal(Level level, object sender, string funcName, string message, int step, int numSteps, params object[] args)
        {
            Utils.ThrowException(step < 0 || (numSteps > 0 && step > numSteps) ? new ArgumentOutOfRangeException("step") : null);
            Progress(level, sender == null ? mDefaultProgressSender : sender, /*freq=*/1, funcName, message, step, numSteps, args); // throws FormatException
        }

        public void ProgressFast(Level level, object sender, string funcName, string message, int step, int numSteps, params object[] args)
        {
            Utils.ThrowException(step < 0 || (numSteps > 0 && step > numSteps) ? new ArgumentOutOfRangeException("step") : null);
            Progress(level, sender == null ? mDefaultProgressSender : sender, /*freq=*/100, funcName, message, step, numSteps, args); // throws FormatException 
        }

        public void ProgressVeryFast(Level level, object sender, string funcName, string message, int step, int numSteps, params object[] args)
        {
            Utils.ThrowException(step < 0 || (numSteps > 0 && step > numSteps) ? new ArgumentOutOfRangeException("step") : null);
            Progress(level, sender == null ? mDefaultProgressSender : sender, /*freq=*/1000, funcName, message, step, numSteps, args); // throws FormatException 
        }

        public void ProgressNormal(Level level, string funcName, string message, int step, int numSteps, params object[] args)
        {
            ProgressNormal(level, /*sender=*/this, funcName, message, step, numSteps, args); // throws ArgumentOutOfRangeException, FormatException 
        }

        public void ProgressFast(Level level, string funcName, string message, int step, int numSteps, params object[] args)
        {
            ProgressFast(level, /*sender=*/this, funcName, message, step, numSteps, args); // throws ArgumentOutOfRangeException, FormatException 
        }

        public void ProgressVeryFast(Level level, string funcName, string message, int step, int numSteps, params object[] args)
        {
            ProgressVeryFast(level, /*sender=*/this, funcName, message, step, numSteps, args); // throws ArgumentOutOfRangeException, FormatException 
        }
    }
}
