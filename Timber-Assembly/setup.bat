@echo off
set VENV_NAME=venv

if not exist %VENV_NAME% (
    echo Creating virtual environment %VENV_NAME%...
    py -3.9 -m venv %VENV_NAME%
    if ERRORLEVEL 1 (
        echo Error creating virtual environment.
        pause
    )
)

echo Virtual environment %VENV_NAME% is ready.

echo Activating Virtural environment...
call .\venv\Scripts\activate
if ERRORLEVEL 1 (
    echo Error activating virtual environment.
    pause
)

echo Upgrading pip...
python -m pip install --upgrade pip
if ERRORLEVEL 1 (
    echo Error upgrading pip.
    pause
)

echo Installing pytorch, this can take awale...
pip3 install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu117
if ERRORLEVEL 1 (
    echo Error installing Pytorch.
    pause
)
echo Pytorch installed.

echo Installing ML Agents...
pip install mlagents
if ERRORLEVEL 1 (
    echo Error installing ML Agents.
    pause
)
echo ML Agents installed.

echo Installing onnx...
pip3 install onnx
if ERRORLEVEL 1 (
    echo Error installing onnx.
    pause
)
echo onnx installed.

echo Downgrading protobuf to 3.20 for stability...
pip install protobuf==3.20.*
if ERRORLEVEL 1 (
    echo Error downgrading protobuf to 3.20.
    pause
)
echo Downgraded protobuf to 3.20.

pause
