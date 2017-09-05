using System;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;

namespace ALE.WaitTask.Helper
{
    public class XmlModificator
    {
        public XmlElement NodeToLoad { get; set; }
        public XmlDocument DocToSave { get; set; }
        public IDTSInfoEvents InfoEvents { get; set; }

        public WaitTaskMain WaitTask { get; set; }
        public XmlModificator(IDTSInfoEvents infoEvents, WaitTaskMain waitTask)
        {
            this.InfoEvents = infoEvents;
            this.WaitTask = waitTask;
        }

        public const string XMLROOT = "WaitTaskData";
        public const string XMLATT_WAITUNTIL_MIN = "WaitUntilTimeMinutes";
        public const string XMLATT_WAITUNTIL_H = "WaitUntilTimeHours";
        public const string XMLATT_SLEEPTIME = "SleepTime";
        public const string XMLATT_DONOTHING = "DoNothing";

        public const string XMLATT_SQLMAXWAITTIME = "MaximumWaitTime";
        public const string XMLATT_SQLREPETITIONTIME = "RepetitionFrequency";
        public const string XMLNODE_SQLCONN = "SQLConnection";
        public const string XMLNODE_SQLCHECKSTMT = "SQLCheckStatement";

        XmlElement elementRoot;
        XmlNode propertyNode;
        XmlAttribute propertyAtt;

        public void SaveXml(XmlDocument doc)
        {
            this.DocToSave = doc;

            CreateRoot(XMLROOT);

            if (WaitTask.DoNothingAndContinue)
                this.AddAttribute(XMLATT_DONOTHING, WaitTask.DoNothingAndContinue.ToString());

            if (WaitTask.HasWaitUntilTime)
            {
                this.AddAttribute(XMLATT_WAITUNTIL_MIN, WaitTask.WaitUntilTimeInternal.Minutes.ToString());
                this.AddAttribute(XMLATT_WAITUNTIL_H, WaitTask.WaitUntilTimeInternal.Hours.ToString());
            }

            if (WaitTask.HasSleepTime)
                this.AddAttribute(XMLATT_SLEEPTIME, WaitTask.SleepTimeInMinutes.ToString());

            if (WaitTask.HasSQLCheckStatement)
                this.AddNode(XMLNODE_SQLCHECKSTMT, WaitTask.SQLCheckStatement);

            if (WaitTask.HasSQLConnection)
                this.AddNode(XMLNODE_SQLCONN, WaitTask.SQLConnectionID);

            if (WaitTask.HasSQLRepetitionFrequencyInMinutes)
                this.AddAttribute(XMLATT_SQLREPETITIONTIME, WaitTask.SQLRepetitionFrequencyInMinutes.ToString());

            if (WaitTask.HasMaximumSQLWaitTime)
                this.AddAttribute(XMLATT_SQLMAXWAITTIME, WaitTask.MaximumSQLWaitTime.ToString());

            DocToSave.AppendChild(elementRoot);
        }

        private void CreateRoot(string rootName)
        {
            elementRoot = DocToSave.CreateElement(rootName);
        }

        public void AddAttribute(string attName, string attValue)
        {
            propertyAtt = DocToSave.CreateAttribute(attName);
            propertyAtt.Value = attValue;
            elementRoot.Attributes.Append(propertyAtt);
        }

        public void AddNode(string nodeName, string nodeValue)
        {
            propertyNode = DocToSave.CreateNode(XmlNodeType.Element, nodeName, String.Empty);
            propertyNode.InnerText = nodeValue;
            elementRoot.AppendChild(propertyNode);
        }

        public void LoadXml(XmlElement node)
        {
            try
            {
                this.NodeToLoad = node;

                if (NodeToLoad.Name != XMLROOT)
                    throw new Exception("Das XML ist ungültig (erwartet wurde <" + XMLROOT + ">)");

                if (NodeToLoad.Attributes[XMLATT_DONOTHING] != null)
                    WaitTask.DoNothingAndContinue = bool.Parse(NodeToLoad.Attributes[XMLATT_DONOTHING].Value);

                if (NodeToLoad.Attributes[XMLATT_WAITUNTIL_H] != null && NodeToLoad.Attributes[XMLATT_WAITUNTIL_MIN] != null)
                    WaitTask.WaitUntilTimeInternal = new Time(int.Parse(NodeToLoad.Attributes[XMLATT_WAITUNTIL_H].Value), int.Parse(NodeToLoad.Attributes[XMLATT_WAITUNTIL_MIN].Value));

                if (NodeToLoad.Attributes[XMLATT_SLEEPTIME] != null)
                    WaitTask.SleepTimeInMinutes = int.Parse(NodeToLoad.Attributes[XMLATT_SLEEPTIME].Value);

                if (NodeToLoad.Attributes[XMLATT_SQLMAXWAITTIME] != null)
                    WaitTask.MaximumSQLWaitTime = int.Parse(NodeToLoad.Attributes[XMLATT_SQLMAXWAITTIME].Value);

                if (NodeToLoad.Attributes[XMLATT_SQLREPETITIONTIME] != null)
                    WaitTask.SQLRepetitionFrequencyInMinutes = int.Parse(NodeToLoad.Attributes[XMLATT_SQLREPETITIONTIME].Value);

                if (NodeToLoad.HasChildNodes)
                {
                    for (int i = 0; i < NodeToLoad.ChildNodes.Count; i++)
                    {
                        switch (NodeToLoad.ChildNodes[i].Name)
                        {
                            case XMLNODE_SQLCHECKSTMT: WaitTask.SQLCheckStatement = NodeToLoad.ChildNodes[i].InnerText; break;
                            case XMLNODE_SQLCONN: WaitTask.SQLConnectionID = NodeToLoad.ChildNodes[i].InnerText; break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Fehler beim Laden des A.LE WaitTask!(" + e.Message + ")", e);
            }
        }
    }
}
