mkdir models
mkdir output
wget -O models/model.onnx  https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/model.onnx 
wget -O models/config.json  https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/config.json

pip install -r requirements.txt
git clone https://github.com/xfgryujk/blivedm.git
git clone https://github.com/CjangCjengh/MoeGoe.git
pip install -r blivedm/requirements.txt
pip install -r MoeGoe/requirements.txt

cp config.ini my_config.ini
cp sensitive_words.txt my_sensitive_words.txt