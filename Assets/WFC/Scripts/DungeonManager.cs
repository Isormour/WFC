using UnityEngine;

namespace WFC
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance;
        public ConditionsConfig conditionConfig;

        bool initialized = false;
        private void Awake()
        {
            Initialize();
        }
        public void Initialize()
        {
            if (initialized)
                return;
            initialized = true;
            if (Instance != this)
            {
                if (Instance != null)
                    Destroy(Instance.gameObject);
            }
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public static void ColorCellObject(Color col, Cell item)
        {
            MeshRenderer[] rends = item.CellObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var rend in rends)
            {
                rend.material.color = rend.material.color * col;
            }
        }
    }
}
