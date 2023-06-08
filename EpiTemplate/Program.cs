using System.Text.RegularExpressions;
using EpiTemplate;

Regex regex = new Regex("^(.*)=(.*)$");
Dictionary<string, string> configParameters = new();

void PopulateParameter()
{
    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];

        // Check if the argument starts with a dash (-)
        if (!arg.StartsWith("-")) continue;
        // Extract the parameter name by removing the dash (-) and any subsequent hyphens
        var parameter = arg.TrimStart('-').TrimStart('-');

        // Make sure there is a subsequent argument
        if (i + 1 < args.Length)
        {
            // Store the parameter-value pair in the dictionary
            configParameters[parameter] = args[i + 1];
        }
    }
}

void LoadConfigFile(string filePath)
{
    if (File.Exists(filePath))
    {
        var configContent = File.ReadAllLines(filePath);
        foreach (var config in configContent)
        {
            if (regex.IsMatch(config))
            {
                var match = regex.Match(config);
                configParameters[match.Groups[1].Value] = match.Groups[2].Value;
            }
        }
    }
}

void ShowHelp()
{
    Console.WriteLine("Usage:\r\n");
    Console.WriteLine("EpiTemplate -c configFile -p projectName -block blockName -create-view-model true/false -content-path baseContentDirectory " +
                      "-controller-path baseControllerDirectory -view-model-path viewModelBaseDirectory " +
                      "-view-path viewBaseDirectory");
    Console.WriteLine("\tc: Configuration file path, it will override other config (Optional)");
    // Console.WriteLine("\tp: Configuration file path, it will override other config (Optional)");
}

PopulateParameter();
if (configParameters.TryGetValue(CommandParameter.ConfigFile, out var configPath))
{
    LoadConfigFile(configPath);
}

TemplateProcessor processor = new(configParameters[CommandParameter.ContentFolder],
    configParameters[CommandParameter.ControllerFolder],
    configParameters[CommandParameter.ViewModelFolder], configParameters[CommandParameter.ViewFolder]);
bool hasViewModel = configParameters.ContainsKey(CommandParameter.CreateViewModel);
if (configParameters.TryGetValue(CommandParameter.CreateViewModel, out var configVal) &&
    bool.TryParse(configVal, out hasViewModel))
{
    
}
await processor.Process(configParameters[CommandParameter.ProjectName], configParameters[CommandParameter.BlockName], hasViewModel);