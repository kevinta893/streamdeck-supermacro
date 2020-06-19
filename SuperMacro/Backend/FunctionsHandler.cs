using BarRaider.SdTools;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMacro.Backend
{
    public static class FunctionsHandler
    {
        #region Private Members

        private delegate string FunctionPointer(string[] args);
        #endregion

        //---------------------------------------------------
        //          BarRaider's Hall Of Fame
        // CyberlightGames - 1 Gifted Subs
        //---------------------------------------------------
        //          Honorary Mention
        // Marbles On Stream winner: xntss
        //---------------------------------------------------
        public static void HandleFunctionRequest(string functionData, Dictionary<string, string> dicVariables)
        {
            if (dicVariables == null)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"HandleFunctionRequest Dictionary is null");
                return;
            }

            string[] parameters = functionData.Split(':');
            if (parameters.Length < 3)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"HandleFunctionRequest Invalid number of params {functionData}");
                return;
            }

            // Handle the incoming parameters
            string functionName = parameters[0].ToUpperInvariant();
            string outputVariable = parameters[1].ToUpperInvariant();
            for (int currentParam = 2; currentParam < parameters.Length; currentParam++)
            {
                parameters[currentParam] = ExtendedMacroHandler.TryExtractVariable(parameters[currentParam]);
            }
            
            // Try and figure out the right function to call
            FunctionPointer requestedFunction = RetreiveRequestedFunction(functionName);
            if (requestedFunction == null)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"HandleFunctionRequest Invalid function name {functionName}");
                return;
            }

            string result = requestedFunction(parameters.Skip(2).ToArray());
            if (result == null)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"HandleFunctionRequest Invalid result from {functionName}");
                return;
            }

            dicVariables[outputVariable] = result;
        }

        private static FunctionPointer RetreiveRequestedFunction(string functionName)
        {
            functionName = functionName.ToUpperInvariant();

            switch (functionName)
            {
                case "ADD":
                    return Add;
                case "SUB":
                    return Sub;
                case "MUL":
                    return Multiply;
                case "DIV":
                    return Divide;
                case "RANDOM":
                    return GetRandom;
                case "CONCAT":
                    return Concat;
            }
            return null;
        }

        private static bool ExtractTwoNumbers(string[] args, out double num1, out double num2)
        {
            num1 = num2 = 0;
            if (args.Length != 2)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest ExtractTwoNumbers: Invalid number of parameters");
                return false;
            }

            if (!Double.TryParse(args[0], out num1))
            {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleFunctionRequest ExtractTwoNumbers: First param is not a valid number {args[0]}");
                    return false;
            }

            if (!Double.TryParse(args[1], out num2))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleFunctionRequest ExtractTwoNumbers: Second param is not a valid number {args[1]}");
                return false;
            }

            // If we reached this point, both num1 and num2 are valid
            return true;
        }

        private static string Add(string[] args)
        {
            double num1, num2;
            if (!ExtractTwoNumbers(args, out num1, out num2))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest Add: ExtractTwoNumbers failed");
                return null;
            }

            return (num1 + num2).ToString();
        }

        private static string Sub(string[] args)
        {
            double num1, num2;
            if (!ExtractTwoNumbers(args, out num1, out num2))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest Sub: ExtractTwoNumbers failed");
                return null;
            }

            return (num1 - num2).ToString();
        }

        private static string Multiply(string[] args)
        {
            double num1, num2;
            if (!ExtractTwoNumbers(args, out num1, out num2))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest Multiply: ExtractTwoNumbers failed");
                return null;
            }

            return (num1 * num2).ToString();
        }

        private static string Divide(string[] args)
        {
            double num1, num2;
            if (!ExtractTwoNumbers(args, out num1, out num2))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest Divide: ExtractTwoNumbers failed");
                return null;
            }

            if (num2 == 0)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest Divide: Division by zero");
                return null;
            }
            return (num1 / num2).ToString();
        }

        private static string GetRandom(string[] args)
        {
            double num1, num2;
            if (!ExtractTwoNumbers(args, out num1, out num2))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest GetRandom: ExtractTwoNumbers failed");
                return null;
            }

            if (num2 == 0)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "HandleFunctionRequest GetRandom: Division by zero");
                return null;           
            }

            return RandomGenerator.Next((int)num1, (int)num2).ToString();
        }

        private static string Concat(string[] args)
        {
            return String.Join("", args);
        }



    }
}
