using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCtoCardUiFactory : MonoBehaviour
{
    public static GCtoCardUiFactory _instance;

    void Awake()
    {
        // 确保只有一个实例存在
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 如果已存在一个实例而且不是当前实例，则销毁当前GameObject
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    [Header("卡牌的UI实例们")]
    public GameObject NormalMove_L01_01_prefab;
    public GameObject NormalMove_L01_02_prefab;
    public GameObject NormalAdd_ZXL_prefab;
    public GameObject NormalAdd_TC_prefab;
    public GameObject NormalADD_JS_prefab;
    public GameObject DoubleBuffer_prefab;
    public GameObject ExVBooster_prefab;

    public GameObject NormalMove_L02_01_prefab;
    public GameObject NormalMove_L02_02_prefab;
    public GameObject NormalAdd_FD_prefab;
    public GameObject ImmuneCheckLocker_prefab;
    public GameObject FKAllergySignal_prefab;
    public GameObject TargetTransport_prefab;
    public GameObject OxidativeBurst_prefab;
    public GameObject BloodActivator_prefab;

    public GameObject NormalMove_L03_01_prefab;
    public GameObject NormalMove_L03_02_prefab;
    public GameObject NormalAdd_JXB_prefab;
    public GameObject TCGeneEditing_prefab;
    public GameObject ZXLActivator_prefab;
    public GameObject BCellForcedSplit_prefab;
    public GameObject AntiBodyModifier_prefab;
    public GameObject CellBindingCapsule_prefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject generalCardToUiFactory(GeneralCard gCard)
    {
        switch (gCard)
        {
            case NormalMove_Level01_01:
                return NormalMove_L01_01_prefab;

            case NormalMove_Level01_02:
                return NormalMove_L01_02_prefab;

            case NormalAdd_ZXL:
                return NormalAdd_ZXL_prefab;

            case NormalAdd_TC:
                return NormalAdd_TC_prefab;

            case NormalAdd_JS:
                return NormalADD_JS_prefab;

            case DoubleBuffer:
                return DoubleBuffer_prefab;

            case ExtraVitaminBooster:
                return ExVBooster_prefab;

            case NormalMove_Level02_01:
                return NormalMove_L02_01_prefab;

            case NormalMove_Level02_02:
                return NormalMove_L02_02_prefab;

            case NormalAdd_FD:
                return NormalAdd_FD_prefab;

            case ImmuneCheckLocker:
                return ImmuneCheckLocker_prefab;

            case FKAllergySignal:
                return FKAllergySignal_prefab;

            case TargetTransport:
                return TargetTransport_prefab;

            case OxidativeBurst:
                return OxidativeBurst_prefab;

            case BloodActivator:
                return BloodActivator_prefab;

            case NormalAdd_JXB:
                return NormalAdd_JXB_prefab;

            case TCGeneEditing:
                return TCGeneEditing_prefab;

            case ZXLActivator:
                return ZXLActivator_prefab;

            case BCellForcedSplit:
                return BCellForcedSplit_prefab;

            case AntiBodyModifier:
                return AntiBodyModifier_prefab;

            case CellBindingCapsule:
                return CellBindingCapsule_prefab;
        }
        return null;
    } 

}
