 


import torch
from light_cnns import mbv1

sourcePath='/app/live_vits_chatgpt/models/G_latest.pth'
outPath='/app/live_vits_chatgpt/models/G_latest.onnx'
 
def main():
    model = mbv1()
    imput = torch.randn(1,3,224,224)
    torch.onnx.export(model, input, "mobilenetV2.onnx", verbose=True, opset_version=11, export_params=True)
 
if __name__ == '__main__':
    main()

