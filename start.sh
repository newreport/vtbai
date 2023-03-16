# ENV::Ubuntu 22.04 LTS

# 安装 gcc 环境
mkdir models
mkdir output
wget -O models/model.onnx  https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/model.onnx 
wget -O models/config.json  https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/config.json

pip install -r requirements.txt
git submodule update --init --recursive
pip install -r blivedm/requirements.txt

cp config.ini my_config.ini
cp sensitive_words.txt my_sensitive_words.txt