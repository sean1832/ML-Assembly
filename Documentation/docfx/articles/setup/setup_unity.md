# Setup Unity
## Automatic install relevant python packages (Recommended)
1. Navigate to `ML-Assembly\Timber-Assembly`.
2. Run `setup.bat` by double clicking on it.
> This will automatically install all the relevant python packages and their dependencies. It will also create a virtual environment called `venv` in the `Timber-Assembly` folder.
> If automatic installation fails, please try manual installation below and submit an issue [here](https://github.com/sean1832/ML-Assembly/issues).

## Manual install relevant python packages
1. Open a terminal and navigate to `ML-Assembly\Timber-Assembly`.
2. Create a virtual environment by running the following command:
```bash
py -3.9 -m venv venv
```
3. Activate the virtual environment by running the following command:
```bash
venv\Scripts\activate
```
4. Upgrade pip by running the following command:
```bash
python -m pip install --upgrade pip
```
5. Install pytorch by running the following command:
```bash
pip3 install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu117
```

6. Install **ML Agent** python package by running the following command:
```bash
pip install mlagents
```
7. Install **onnx** python package by running the following command:
```bash
pip3 install onnx
```
8. Downgrade **protobuf** to version 3.20 for stability by running the following command:
```bash
pip install protobuf==3.20
```


## Setup Unity project
1. Open Unity Hub.
2. Under `Projects`, click on `Open`.
3. Navigate to `ML-Assembly\Timber-Assembly` and click `Select Folder`.
> Unity will take a while to load the project for the first time.

## Next steps
- [**Setup Grasshopper**](setup_grasshopper.md)