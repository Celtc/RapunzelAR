using UnityEngine;
using System.Collections;

public class PrimitiveCreatorUtil
{
    //Crea un plano de caracteristicas definidas
    public static GameObject CreatePlane(string name,
                                          Vector3 position,
                                          Quaternion rotation,
                                          Orientation orientation,
                                          int widthSegments,
                                          int lengthSegments,
                                          float width,
                                          float length,
                                          PlaneAnchorPoint anchor,
                                          bool addCollider,
                                          Material material)
    {
        GameObject plane = new GameObject();

        //Establece el nombre
        if (!string.IsNullOrEmpty(name))
            plane.name = name;
        else
            plane.name = "Plane";

        //Lo acomoda
        plane.transform.position = position;
        plane.transform.rotation = rotation;

        //Define el anchor
        Vector2 anchorOffset;
        switch (anchor)
        {
            case PlaneAnchorPoint.TopLeft:
                anchorOffset = new Vector2(-width / 2.0f, length / 2.0f);
                break;
            case PlaneAnchorPoint.TopHalf:
                anchorOffset = new Vector2(0.0f, length / 2.0f);
                break;
            case PlaneAnchorPoint.TopRight:
                anchorOffset = new Vector2(width / 2.0f, length / 2.0f);
                break;
            case PlaneAnchorPoint.RightHalf:
                anchorOffset = new Vector2(width / 2.0f, 0.0f);
                break;
            case PlaneAnchorPoint.BottomRight:
                anchorOffset = new Vector2(width / 2.0f, -length / 2.0f);
                break;
            case PlaneAnchorPoint.BottomHalf:
                anchorOffset = new Vector2(0.0f, -length / 2.0f);
                break;
            case PlaneAnchorPoint.BottomLeft:
                anchorOffset = new Vector2(-width / 2.0f, -length / 2.0f);
                break;
            case PlaneAnchorPoint.LeftHalf:
                anchorOffset = new Vector2(-width / 2.0f, 0.0f);
                break;
            case PlaneAnchorPoint.Center:
            default:
                anchorOffset = Vector2.zero;
                break;
        }

        //Agrega el mesh renderer (se encarga de la renderizacion de los mesh)
        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        plane.AddComponent(typeof(MeshRenderer));

        //Crea el mesh en cuestion
        var mesh = new Mesh();
        mesh.name = plane.name;

        int hCount2 = widthSegments + 1;
        int vCount2 = lengthSegments + 1;
        int numTriangles = widthSegments * lengthSegments * 6;
        int numVertices = hCount2 * vCount2;

        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];

        int index = 0;
        float uvFactorX = 1.0f / widthSegments;
        float uvFactorY = 1.0f / lengthSegments;
        float scaleX = width / widthSegments;
        float scaleY = length / lengthSegments;
        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, 0.0f, y * scaleY - length / 2f - anchorOffset.y);
                }
                else
                {
                    vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, y * scaleY - length / 2f - anchorOffset.y, 0.0f);
                }
                uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
            }
        }

        index = 0;
        for (int y = 0; y < lengthSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                triangles[index] = (y * hCount2) + x;
                triangles[index + 1] = ((y + 1) * hCount2) + x;
                triangles[index + 2] = (y * hCount2) + x + 1;

                triangles[index + 3] = ((y + 1) * hCount2) + x;
                triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                triangles[index + 5] = (y * hCount2) + x + 1;
                index += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        //Agrega el collider?
        if (addCollider)
            plane.AddComponent(typeof(BoxCollider));

        //Asigna un material?
        if (material != null)
            plane.renderer.material = material;

        return plane;
    }

    //Crea un plano de componentes hexagonales de caracteristicas definidas
    public static GameObject CreateHexLattice(string name,
                                               Vector3 position,
                                               Quaternion rotation,
                                               Orientation orientation,
                                               int widthSegments,
                                               int lengthSegments,
                                               float width,
                                               float length,
                                               PlaneAnchorPoint anchor,
                                               bool addCollider,
                                               Material material)
    {
        GameObject hexLattice = new GameObject();

        if (!string.IsNullOrEmpty(name))
            hexLattice.name = name;
        else
            hexLattice.name = "hexLattice";

        //Lo acomoda
        hexLattice.transform.position = position;
        hexLattice.transform.rotation = rotation;

        Vector2 anchorOffset;
        switch (anchor)
        {
            case PlaneAnchorPoint.TopLeft:
                anchorOffset = new Vector2(-width / 2.0f, length / 2.0f);
                break;
            case PlaneAnchorPoint.TopHalf:
                anchorOffset = new Vector2(0.0f, length / 2.0f);
                break;
            case PlaneAnchorPoint.TopRight:
                anchorOffset = new Vector2(width / 2.0f, length / 2.0f);
                break;
            case PlaneAnchorPoint.RightHalf:
                anchorOffset = new Vector2(width / 2.0f, 0.0f);
                break;
            case PlaneAnchorPoint.BottomRight:
                anchorOffset = new Vector2(width / 2.0f, -length / 2.0f);
                break;
            case PlaneAnchorPoint.BottomHalf:
                anchorOffset = new Vector2(0.0f, -length / 2.0f);
                break;
            case PlaneAnchorPoint.BottomLeft:
                anchorOffset = new Vector2(-width / 2.0f, -length / 2.0f);
                break;
            case PlaneAnchorPoint.LeftHalf:
                anchorOffset = new Vector2(-width / 2.0f, 0.0f);
                break;
            case PlaneAnchorPoint.Center:
            default:
                anchorOffset = Vector2.zero;
                break;
        }

        MeshFilter meshFilter = (MeshFilter)hexLattice.AddComponent(typeof(MeshFilter));
        hexLattice.AddComponent(typeof(MeshRenderer));

        var mesh = new Mesh();
        mesh.name = hexLattice.name;

        int hCount2 = widthSegments + 1;
        int vCount2 = lengthSegments + 1;
        int numTriangles = widthSegments * lengthSegments * 6;
        int numVertices = hCount2 * vCount2;

        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];

        int index = 0;
        float uvFactorX = 1.0f / widthSegments;
        float uvFactorY = 1.0f / lengthSegments;
        float scaleX = width / widthSegments;
        float scaleY = length / lengthSegments;
        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x - (y % 2 - 1) * scaleX * .5f, 0.0f, y * scaleY - length / 2f - anchorOffset.y);
                }
                else
                {
                    vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x - (y % 2 - 1) * scaleX * .5f, y * scaleY - length / 2f - anchorOffset.y, 0.0f);
                }
                uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
            }
        }

        index = 0;
        for (int y = 0; y < lengthSegments; y++)
        {
            if (y % 2 == 0)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index] = (y * hCount2) + x;
                    triangles[index + 1] = ((y + 1) * hCount2) + x;
                    triangles[index + 2] = ((y + 1) * hCount2) + x + 1;


                    triangles[index + 3] = (y * hCount2) + x;
                    triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                    triangles[index + 5] = (y * hCount2) + x + 1;
                    index += 6;
                }
            }
            else
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index] = (y * hCount2) + x;
                    triangles[index + 1] = ((y + 1) * hCount2) + x;
                    triangles[index + 2] = (y * hCount2) + x + 1;

                    triangles[index + 3] = ((y + 1) * hCount2) + x;
                    triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                    triangles[index + 5] = (y * hCount2) + x + 1;
                    index += 6;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.Optimize();


        meshFilter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        if (addCollider)
            hexLattice.AddComponent(typeof(BoxCollider));

        if (material != null)
            hexLattice.renderer.material = material;

        return hexLattice;
    }

    //Crea un diamante formado por 8 triangulos con las caracteristicas dadas
    public static GameObject CreateOctahedron(string name,
                                               Vector3 position,
                                               Quaternion rotation,
                                               Orientation orientation,
                                               float height,
                                               float radius,
                                               OctahedronAnchorPoint anchor,
                                               bool addCollider,
                                               Material material)
    {
        GameObject octahedron = new GameObject();

        //Establece el nombre
        if (!string.IsNullOrEmpty(name))
            octahedron.name = name;
        else
            octahedron.name = "Octahedron";

        //Lo acomoda
        octahedron.transform.position = position;
        octahedron.transform.rotation = rotation;

        //Define el anchor
        float anchorOffset;
        switch (anchor)
        {
            case OctahedronAnchorPoint.Bottom:
                anchorOffset = height / 2.0f;
                break;
            case OctahedronAnchorPoint.Top:
                anchorOffset = -height / 2.0f;
                break;
            case OctahedronAnchorPoint.Center:
            default:
                anchorOffset = 0;
                break;
        }

        //Agrega el mesh renderer (se encarga de la renderizacion de los mesh)
        MeshFilter meshFilter = (MeshFilter)octahedron.AddComponent(typeof(MeshFilter));
        octahedron.AddComponent(typeof(MeshRenderer));

        //Crea el mesh en cuestion
        var mesh = new Mesh();
        mesh.name = octahedron.name;

        int numVertices = 6;

        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];

        //Creo los vertices
        float cateto = Mathf.Sqrt(radius / 2.0f);
        vertices[0] = new Vector3(0, -height / 2.0f + anchorOffset, 0); //Bottom
        vertices[1] = new Vector3(0, height / 2.0f + anchorOffset, 0); //Top
        vertices[2] = new Vector3(-cateto, anchorOffset, -cateto);
        vertices[3] = new Vector3(cateto, anchorOffset, -cateto);
        vertices[4] = new Vector3(-cateto, anchorOffset, cateto);
        vertices[5] = new Vector3(cateto, anchorOffset, cateto);

        //Asigno los UVs
        uvs[0] = new Vector2(0.5f, 0); //Bottom
        uvs[1] = new Vector2(0.5f, 1); //Top
        uvs[2] = new Vector2(0, 0.5f);
        uvs[3] = new Vector2(1, 0.5f);
        uvs[4] = new Vector2(1, 0.5f);
        uvs[5] = new Vector2(0, 0.5f);

        //Creo los tringulos
        int[] triangles = new int[] { 0, 2, 3, 0, 4, 2, 0, 5, 4, 0, 3, 5, 2, 1, 3, 4, 1, 2, 5, 1, 4, 3, 1, 5 };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        //Agrega el collider?
        if (addCollider)
        {
        }

        //Asigna un material?
        if (material != null)
            octahedron.renderer.material = material;

        return octahedron;
    }

    //Crea un prisma rectangular (cubo de lados no iguales)
    public static GameObject CreateRectangularPrism(string name,
                                                     Vector3 center,
                                                     Quaternion rotation,
                                                     Vector3 size,
                                                     bool addCollider,
                                                     bool eightVertex,
                                                     Material material)
    {
        GameObject prism = new GameObject();

        //Establece el nombre
        if (!string.IsNullOrEmpty(name))
            prism.name = name;
        else
            prism.name = "RectangularPrism";

        //Lo acomoda
        prism.transform.position = center;
        prism.transform.rotation = rotation;

        //Agrega el mesh renderer (se encarga de la renderizacion de los mesh)
        MeshFilter meshFilter = (MeshFilter)prism.AddComponent(typeof(MeshFilter));
        prism.AddComponent(typeof(MeshRenderer));

        //Crea el mesh en cuestion
        var mesh = new Mesh();
        mesh.name = prism.name;

        Vector2[] uvs;
        Vector3[] vertices;
        int[] triangles;

        //8 vertices?
        if (eightVertex)
        {
            int numVertices = 8;
            uvs = new Vector2[numVertices];
            vertices = new Vector3[numVertices];

            //Creo los vertices
            var extents = size * 0.5f;
            vertices[0] = new Vector3(-extents.x, -extents.y, -extents.z);
            vertices[1] = new Vector3(extents.x, -extents.y, -extents.z);
            vertices[2] = new Vector3(extents.x, -extents.y, extents.z);
            vertices[3] = new Vector3(-extents.x, -extents.y, extents.z);
            vertices[4] = new Vector3(-extents.x, extents.y, -extents.z);
            vertices[5] = new Vector3(extents.x, extents.y, -extents.z);
            vertices[6] = new Vector3(extents.x, extents.y, extents.z);
            vertices[7] = new Vector3(-extents.x, extents.y, extents.z);

            //Asigno los UVs
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(1f, 0f);
            uvs[2] = new Vector2(0f, 0f);
            uvs[3] = new Vector2(1f, 0f);
            uvs[4] = new Vector2(0f, 1f);
            uvs[5] = new Vector2(1f, 1f);
            uvs[6] = new Vector2(0f, 1f);
            uvs[7] = new Vector2(1f, 1f);

            //Creo los tringulos
            triangles = new int[] { 0, 4, 5, 5, 1, 0, 1, 5, 6, 6, 2, 1, 2, 6, 7, 7, 3, 2, 3, 7, 4, 4, 0, 3, 4, 7, 6, 6, 5, 4, 0, 1, 2, 2, 3, 0 };
        }
        else
        {
            //TODO Falta implementar que el metodo soporte la creacion de cubos con 24 vertices
            return null;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        //Agrega el collider?
        if (addCollider)
        {
        }

        //Asigna un material?
        if (material != null)
            prism.renderer.material = material;

        return prism;
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum OctahedronAnchorPoint
    {
        Bottom,
        Top,
        Center
    }

    public enum PlaneAnchorPoint
    {
        TopLeft,
        TopHalf,
        TopRight,
        RightHalf,
        BottomRight,
        BottomHalf,
        BottomLeft,
        LeftHalf,
        Center
    }
}
