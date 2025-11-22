
file_path = "ProjectSettings/ProjectSettings.asset"

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Replace empty object with specific settings
# We assume it is currently "managedStrippingLevel: {}"
if "managedStrippingLevel: {}" in content:
    new_content = content.replace("managedStrippingLevel: {}", "managedStrippingLevel:\n    Android: 3\n    Standalone: 3")
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    print("Updated Managed Stripping Level to High")
else:
    print("Could not find managedStrippingLevel: {} to replace. It might be already set.")

