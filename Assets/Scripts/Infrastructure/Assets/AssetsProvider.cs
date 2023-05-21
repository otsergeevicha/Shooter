using Services.Assets;
using UnityEngine;

namespace Infrastructure.Assets
{
    public class AssetsProvider : IAssetsProvider
    {
        public GameObject InstantiateEntity(string path)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }
    }
}