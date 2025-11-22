import os

target_dir = "Assets/Asset game"

for root, dirs, files in os.walk(target_dir):
    for file in files:
        if file.endswith(".meta"):
            file_path = os.path.join(root, file)
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            
            if "TextureImporter:" in content:
                new_content = content.replace("crunchedCompression: 0", "crunchedCompression: 1")
                # Optional: Reduce max size if needed, but crunch is safer first step
                # new_content = new_content.replace("maxTextureSize: 2048", "maxTextureSize: 1024")
                
                if content != new_content:
                    print(f"Optimizing: {file_path}")
                    with open(file_path, 'w', encoding='utf-8') as f:
                        f.write(new_content)

