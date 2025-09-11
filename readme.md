# .NET 9.0 RoomMesh (.rmesh) To OBJ Converter

Have you ever wanted SCP Containment Breach's room meshes? If so, then don't get your hopes up too much.

This program allows you to select a folder containing .rmesh files (child directories included) and 
convert all of them into a .obj file alongside any relevant material files. You're probably wondering 
what the catch is, and it's the fact that some stuff just blatantly does not work.

# What Works

- Exporting room meshes to OBJ
- Automatic material generation
- Automatic S&Box file generation
- Lights & Spotlights entities (S&Box prefabs only)
- Simple DirectX (.x) model conversion (models with custom templates, e.g. animations, will not work)

# What doesn't work

- Entities
- Triggers
- External Model Animations & Skin renderers

# Credit

It would not have been possible to create this quickly without the [RoomMesh file format information](https://github.com/Koanyaku/godot_rmesh_import/blob/main/docs/rmesh_format_scp-cb.md) 
posted by [Koanyaku](https://github.com/koanyaku) in their 
[Godot RMesh Import addon](https://github.com/Koanyaku/godot_rmesh_import/tree/main) repository.

DirectX model (.x) conversion would not have been possible without [Paul 
Bourke's webpage on Direct-X File Format](https://paulbourke.net/dataformats/directx/#xfilefrm_Template_MeshMaterialList) 