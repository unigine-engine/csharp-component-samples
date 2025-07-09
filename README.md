# UNIGINE C# Samples

This repository contains [C# UNIGINE samples](https://developer.unigine.com/en/docs/latest/sdk/api_samples/cs/?rlang=cpp) showcasing various features and techniques implemented via code. Each sample's description is stored alongside the sample and can be viewed at runtime.

## Requirements

- [**UNIGINE SDK Browser**](https://developer.unigine.com/en/docs/latest/start/installing_sdk?rlang=cpp) (latest version)
- **UNIGINE SDK Community** or **Engineering** edition (**Sim** upgrade supported)
- **Python 3.10** or newer
- **Visual Studio 2022** (recommended)

Check the full list of system requirements at [developer.unigine.com.](https://developer.unigine.com/en/docs/latest/start/requirements?rlang=cpp)

## Running the Samples

1. **Clone or download** this repository.
2. **Run the `download_samples_content.py` file** to automatically download the sample content.
> [!Note]
> Only code files are included in this repository. The script will fetch the necessary assets into a separate content folder.
3. **Open SDK Browser** and make sure you have the latest version.
4. **Add the project to SDK Browser**:
   - Go to the *My Projects* tab.
   - Click *Add Existing*, select the `.project` file from the cloned folder (matching your OS - `*-win-*`/`*-lin-*`, edition, precision), and click *Import Project*.
     
     ![Add Project](https://developer.unigine.com/en/docs/latest/sdk/api_samples/third_party/photon/add_project.png)
> [!NOTE]
> If you're using **UNIGINE SDK *Sim***, select the ***Engineering*** `*-eng-sim-*.project` file when importing the sample. After import, you can upgrade the project to the **Sim** version directly in SDK Browser - just click *Upgrade*, choose the SDK **Sim** version, and adjust any additional settings you want to use in the configuration window that opens.

5. **Repair the project**:
   - After importing, you'll see a **Repair** warning - this is expected, as only essential project files are stored in the Git repository. SDK Browser will restore the rest.
   
   ![Repair Project](https://developer.unigine.com/en/docs/latest/sdk/api_samples/third_party/repair_project.png)
   - Click *Repair* and then *Configure Project*.

6. **Open the project in Visual Studio** (or your preferred IDE).
   - Launch **Visual Studio 2022** and open the `.sln` file
   
7. **Build** and **Run** the project.

### If the sample fails to run:
  - Double-check all setup steps above to make sure nothing was skipped.
  - Ensure the correct `.project` file is used for your platform and SDK edition.
  - Verify your SDK version is not older than the project's specified version.
  - If build errors occur, try right-clicking the project and selecting **Rebuild**.