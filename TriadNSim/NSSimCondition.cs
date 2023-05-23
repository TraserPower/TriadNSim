﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TriadCore;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;
using TriadNSim.Forms;

namespace TriadNSim
{
    public class NSSimCondition : ICondition 
    {
        private SimulationInfo simInfo;
        private List<IPResult> ipResults;

        public NSSimCondition(SimulationInfo simInfo)
        {
            this.simInfo = simInfo;
            ipResults = new List<IPResult>();
        }
        
        public override void DoInitialize() 
        {
            ipResults.Clear();
            int nCount = 0;
            foreach (NetworkObject spyObject in simInfo.SpyNodes)
            {
                if (spyObject.ConnectedIPs.Count == 0)
                    continue;

                foreach (ConnectedIP ip in spyObject.ConnectedIPs)
                {
                    string ipName = ip.IP.Name;
                    IPResult ipRes = null;
                    if (ip.IP.ReturnCode != TriadCompiler.TypeCode.UndefinedType)
                        ipRes = new IPResult(spyObject, ip);
                    string sAssemblyName = "TriadCore";
                    if (!ip.IP.IsStandart)
                    {
                        sAssemblyName = ip.IP.Name;
                        Assembly ass = Assembly.LoadFile(Application.StartupPath + "\\" + sAssemblyName + ".dll");
                    }
                    object obj = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(sAssemblyName, "TriadCore." + ipName);
                    IProcedure iprocedure = obj as IProcedure;
                    AddIProcedure(iprocedure, nCount++);
                    Type ipType = iprocedure.GetType();
                    MethodInfo mi = ipType.GetMethod("RegisterSpyObjects");
                    object[] parameters = null;
                    if (ip.Params.Count > 0)
                    {
                        parameters = new object[ip.Params.Count];
                        for (int i = 0, nParamCount = ip.IP.Params.Count; i < nParamCount; i++)
                        {
                            IPParam param = ip.IP.Params[i];
                            parameters[i] = GetSpyObject(new CoreName(spyObject.Name + "_" + ipName + "_" + ip.Params[i] + i.ToString()));
                        }
                    }
                    mi.Invoke(iprocedure, parameters);
                    ipResults.Add(ipRes);
                }
            }
            InitializeAllIProcedure();
        }

        public void RegisterSpyObjects(SpyObject objectInfo, String formalName)
        {
            RegisterSpyObject(objectInfo, new CoreName(formalName));
            RegisterSpyHandler(objectInfo, DoHandling);
        }
        
        public void GetOutVariables() 
        {
        }
        
        protected override void DoHandling(SpyObject spyObject, Double SystemTime) 
        {
            String message = spyObject.Data;
        }
        
        public void DoProcessing() 
        {
        }


        public override void OnTerminate()
        {
            int nIPCount = GetIProcedureCount();
            for (int i = 0; i < nIPCount; i++)
            {
                IPResult res = ipResults[i];
                if (res != null)
                {
                    IProcedure ip = GetIProcedure(i);
                    Type ipType = ip.GetType();
                    MethodInfo mi = ipType.GetMethod("DoProcessing");
                    object ipRes = mi.Invoke(ip, null);
                    res.Result = ipRes;
                    Simulation.Instance.IPResults.Add(res);
                }
            }
            base.OnTerminate();
        }

        public override Boolean DoCheck(Double SystemTime) 
        {
            //frmMain.Instance.Text = SystemTime.ToString();
            return SystemTime < simInfo.TerminateTime;
        }
    }
}
