using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    public class SapDemo
    {
        dynamic objFuncs;
        string sapSystem = "EDQ";
        string sapAppServer = "10.20.1.39";
        string sapSystemNum = "00";
        string sapClient = "910";
        string sapUser = "WINSHUTADM";
        string sapPassword = "P@ssw0rd";
        string sapDefaultLan = "EN";

        public int LogOn()
        {
            Type sapFuncType = Type.GetTypeFromProgID("SAP.Functions", true);
            objFuncs = Activator.CreateInstance(sapFuncType, false);
            dynamic objConnection = objFuncs.Connect;

            // System Details
            objConnection.system = sapSystem;
            objConnection.Applicationserver = sapAppServer;
            objConnection.SystemNumber = sapSystemNum;

            // Logon details
            objConnection.client = sapClient;
            objConnection.user = sapUser;
            objConnection.Password = sapPassword;
            objConnection.Language = sapDefaultLan;

            //return 0;

            // Silent logon
            if (!objConnection.Logon(0, true))
                return 0;

            return 1;
        }

        public int IsAssigned(int pernr)
        {
            dynamic objFunc = objFuncs.Add("BAPI_EMPLOYEE_CHECKEXISTENCE");
            objFunc.Exports["NUMBER"] = pernr;

            bool status = objFunc.Call();

            if (status)
            {
                dynamic objSalary = objFunc.Imports("SALARY_DATA");
                int salary = Convert.ToInt32(objSalary.Value["SALARY"]);
                return salary;
            }
            else
            {
                return -1;
            }

            /*
             * if Err.number <> 0 then
           XScript.SetVariable "$error", Err.Number & " Srce: " & Err.Source & " Desc: " &  Err.Description
           Err.Clear
           Wscript.Quit
           End If
             */
        }

        public int ReadSalary(int pernr)
        {
            dynamic objFunc = objFuncs.Add("HRWPC_RFC_CP_SALARY_GET");
            objFunc.Exports["PERNR"] = pernr;

            bool status = objFunc.Call();

            if (status)
            {
                dynamic objSalary = objFunc.Imports("SALARY_DATA");
                int salary = Convert.ToInt32(objSalary["SALARY"]);
                return salary;
            }
            else
            {
                return -1;
            }

            /*
             If Err.number <> 0 then
       XScript.SetVariable "$error", Err.Number & " Srce: " & Err.Source & " Desc: " &  Err.Description
       Err.Clear
       Wscript.Quit
       End If
             */
        }
    }