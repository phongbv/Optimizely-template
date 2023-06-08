using System.Text;

namespace EpiTemplate;

public class TemplateProcessor
{
    private readonly string _contentFolderPath;
    private readonly string _controllerFolderPath;
    private readonly string _viewModelFolderPath;
    private readonly string _viewFolderPath;
    private string _guid = Guid.NewGuid().ToString().ToUpper();

    public TemplateProcessor(string contentFolderPath, string controllerFolderPath,
        string viewModelFolderPath,
        string viewFolderPath)
    {
        _contentFolderPath = contentFolderPath ?? throw new ArgumentNullException(nameof(contentFolderPath));
        _controllerFolderPath =
            controllerFolderPath ?? throw new ArgumentNullException(nameof(controllerFolderPath));
        _viewModelFolderPath = viewModelFolderPath;
        _viewFolderPath = viewFolderPath ?? throw new ArgumentNullException(nameof(viewFolderPath));
    }

    public async Task Process(string projectName, string blockName, bool hasViewModel)
    {
        await ProcessInternal(blockName, $"{blockName}.cs", $"{_contentFolderPath}\\{blockName}", $"{projectName}_block.template");
        if (hasViewModel)
        {
            await  ProcessInternal(blockName, $"{blockName}BlockViewModel.cs", $"{_viewModelFolderPath}\\{blockName}",
                $"{projectName}_block_viewmodel.template");
        }

        await ProcessInternal(blockName, $"{blockName}BlockController.cs", $"{_controllerFolderPath}\\{blockName}",
            hasViewModel
                ? $"{projectName}_block_controller_has_viewmodel.template"
                : $"{projectName}_block_controller_no_viewmodel.template");
       await ProcessInternal(blockName, $"Default.cshtml", $"{_viewFolderPath}\\{blockName}Block",
            hasViewModel ? $"{projectName}_view_has_viewmodel.template" : $"{projectName}_view_no_viewmodel.template");
    }

    private async Task ProcessInternal(string blockName, string fileName, string directory, string templateFileName)
    {
        // Normalize path based on current system path separator
        directory = directory.Replace('\\', Path.DirectorySeparatorChar);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(directory, fileName);
        var templateContent = File.ReadAllText(templateFileName);
        var newContent = templateContent.Replace("$BLOCK_NAME$", blockName);
        newContent = newContent.Replace("$GUID$", _guid);
        newContent = newContent.Replace("$GUID$", _guid);
        newContent = newContent.Replace("$BLOCK_FRIENDLY_NAME$", SplitStringByCapitalLetters(blockName));
        await File.WriteAllTextAsync(filePath, newContent);
    }

    private string SplitStringByCapitalLetters(string str)
    {
        StringBuilder builder = new StringBuilder();
        foreach (char c in str) {
            if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
            builder.Append(c);
        }
        return builder.ToString();
    }
}