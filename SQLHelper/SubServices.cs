using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLHelper
{
    internal static class SubServices
    {
        public static bool ValidateParameter(Dictionary<string,dynamic> paramsToTest)
        {
            if (paramsToTest == null)
            {
                return false;
            }

            if (paramsToTest.Count == 0)
            {
                return false;
            }

            foreach (var param in paramsToTest) 
            {
                if (!ValidateString(param.Key))
                {
                    return false;
                }
                if (IsObjectEmpty(param.Key))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool ValidateString(string stringToTest) 
        {
            if (stringToTest == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(stringToTest))
            {
                return false;
            }

            return true;
        }

        public static bool ValidateString(object stringToTest)
        {
            if (stringToTest == null)
            {
                return false;
            }

            if (stringToTest.GetType() != typeof(string))
            {
                return false;
            }

            if (string.IsNullOrEmpty(stringToTest.ToString()))
            {
                return false;
            }

            return true;
        }

        public static bool IsObjectEmpty(object objectToTest)
        {
            if (objectToTest == null)
            {
                return true;
            }

            return false;
        }
    }
}
