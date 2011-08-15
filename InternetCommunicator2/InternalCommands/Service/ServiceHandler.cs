using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace InterComm.InternalCommands
{
    class ServiceHandler
    {
        private int serviceTimeout = 5 * 1000; //5 seconds
        private string lastErrorString = "null";

        public string LastError
        {
            get
            {
                return lastErrorString;
            }
        }

        public ServiceHandler()
        {

        }

        public bool StartService(string serviceName)
        {
            bool result = false;
            try
            {
                ServiceController service = new ServiceController(serviceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(serviceTimeout);
                ServiceControllerStatus serviceStatus = service.Status;
                if (serviceStatus == ServiceControllerStatus.Running || serviceStatus == ServiceControllerStatus.StartPending)
                {
                    string errorMessage = "Service (" + serviceName + ") already running or startPending";
                    throw new InvalidOperationException(errorMessage);
                }
                //serivce can be started
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                result = true;
            }
            catch (Exception ex)
            {
                lastErrorString = ex.Message;
                result = false;
            }

            return result;
        }

        public bool StopService(string serviceName)
        {
            bool result = false;
            try
            {
                ServiceController service = new ServiceController(serviceName);
                TimeSpan timeout = TimeSpan.FromMilliseconds(serviceTimeout);

                if (service.CanStop == false)
                {
                    string errorMessage = "Service can not be stopped!";
                    throw new InvalidOperationException(errorMessage);
                }

                ServiceControllerStatus serviceStatus = service.Status;
                if (serviceStatus == ServiceControllerStatus.Stopped || serviceStatus == ServiceControllerStatus.StopPending)
                {
                    string errorMessage = "Service (" + serviceName + ") already stopped or stopPending";
                    throw new InvalidOperationException(errorMessage);
                }
                //serivce can be started
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                result = true;
            }
            catch (Exception ex)
            {
                lastErrorString = ex.Message;
                result = false;
            }

            return result;
        }

        public bool RestartService(string serviceName)
        {
            bool result = false;
            result = StopService(serviceName);
            if (result == false) return false;
            result = StartService(serviceName);
            if (result == false) return false;

            return result;
        }

        public string GetStatus(string serviceName)
        {
            string serviceStatusString = "null";
            try
            {
                ServiceController service = new ServiceController(serviceName);
                serviceStatusString = service.Status.ToString();
            }
            catch (Exception ex)
            {
                serviceStatusString = ex.Message;
            }
            return serviceStatusString;
        }

    }
}
