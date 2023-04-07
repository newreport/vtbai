pip install -r requirements.txt
git submodule update
git submodule update --init --recursive
git submodule update --remote
pip install -r blivedm/requirements.txt
pip install -r MoeGoe/requirements.txt
python init.py