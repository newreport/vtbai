mkdir models
mkdir output
wget -O models/model.onnx  https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/model.onnx 
wget -O models/config.json  https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/config.json

pip install requirements.txt