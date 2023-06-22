# Update the project to the latest release
This page describes how to update the project to the latest release. You must have already setup the [environment](../setup/setup_env.md), [unity](../setup/setup_unity.md) and [grasshopper](../setup/setup_grasshopper.md) before updating the project.

## Update Class Library
1. Download the [latest release](https://github.com/sean1832/ML-Assembly/releases/latest) of `TimberAssembly-{version number}.zip` and its dependencies. It should be a `.zip` file containing `TimberAssembly.dll` and its dependencies.
2. Unzip the `.zip` file to a folder on your local machine. Replace the existing files in that folder.
3. Re-open grasshopper and navigate to `CompareDLL` to check if the version number is updated.

## Update project files via git (recommended)
1. Ensure that you have [git](https://git-scm.com/downloads) or [GitHub Desktop](https://desktop.github.com/) installed.
2. Open a terminal and navigate to your local `ML-Assembly` folder.
3. Enter the following command:
```bash
git pull
```
> If you are using GitHub Desktop, you can also use the GUI to pull the latest changes.
> [See here](https://docs.github.com/en/desktop/contributing-and-collaborating-using-github-desktop/adding-and-cloning-repositories/cloning-and-forking-repositories-from-github-desktop) for more information.

## Update project files manually
1. Download the [latest release](https://github.com/sean1832/ML-Assembly/releases/latest) of `Source code.zip`. It should be a `.zip` file containing all the project files.
2. Unzip the `.zip` file to a folder on your local machine. Replace or delete the existing files in that folder. You can also create a new folder and unzip files to that folder.
3. Set up the project again. See:
- [Setup Unity](../setup/setup_unity.md)
- [Setup Grasshopper](../setup/setup_grasshopper.md)