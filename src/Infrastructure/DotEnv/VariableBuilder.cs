using Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DotEnv;

public class VariableBuilder
{
   
    public static string GetVariable(string variableName)
    {
        string path = Path
                        .Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, EnvFileConstants.DEV_ENV_FILE_NAME);

        if (!File.Exists(path) || string.IsNullOrWhiteSpace(variableName))
            return string.Empty;

        string variable = GetVariableFromEnvFile(variableName, path);

        return variable;
    }

    public static string GetVariableFromEnvFile(string variableName, string envFilePath)
    {

        string value = string.Empty;

        foreach (var line in File.ReadAllLines(envFilePath))
        {
            var parts = line.Split(
                                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts[0].ToString().ToLower().Contains(variableName.ToLower()))
            {
                value = line.Replace(variableName + "=", string.Empty);
                break;
            }
        }

        return value;
    }

    
}
