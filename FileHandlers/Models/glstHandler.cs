using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;

namespace SSX_Modder.FileHandlers
{
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

    public class glstHandler
    {
        public static void SaveglST(string Output, SSXMPFModelHandler.MPFModelHeader modelHeader)
        {
            var mesh = new MeshBuilder<VERTEX>(modelHeader.FileName);
            //Make Materials

            List<MaterialBuilder> materialBuilders = new List<MaterialBuilder>();

            for (int i = 0; i < modelHeader.materialDataList.Count; i++)
            {
                var TempVar = modelHeader.materialDataList[i];
                var material1 = new MaterialBuilder(TempVar.Name)
                .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(TempVar.X, TempVar.Y, TempVar.Z, 1));
                materialBuilders.Add(material1);
                var prim = mesh.UsePrimitive(materialBuilders[materialBuilders.Count-1]);
            }


            for (int i = 0; i < modelHeader.staticMesh.Count; i++)
            {
                var Data = modelHeader.staticMesh[i];
                int tempInt = Data.ChunkID;
                int MatId = modelHeader.chunks[tempInt].MaterialID;
                var prim = mesh.UsePrimitive(materialBuilders[MatId]);

                for (int b = 0; b < Data.faces.Count; b++)
                {
                    var Face = Data.faces[b];
                    prim.AddTriangle(new VERTEX(Face.V1.X, Face.V1.Y, Face.V1.Z), new VERTEX(Face.V2.X, Face.V2.Y, Face.V2.Z), new VERTEX(Face.V3.X, Face.V3.Y, Face.V3.Z));
                }

            }

            var scene = new SharpGLTF.Scenes.SceneBuilder();

            scene.AddRigidMesh(mesh, Matrix4x4.Identity);

            // save the model in different formats

            var model = scene.ToGltf2();
            model.SaveAsWavefront(Output + "/mesh.obj");
            model.SaveGLB(Output+ "/mesh.glb");
            model.SaveGLTF(Output + "/mesh.gltf");
        }
    }
}
