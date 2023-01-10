using UnityEngine;
using UnityEngine.UI;
using System;
using JKFrame;
namespace R
{
 
    public static class DefaultLocalGroup
    {
 
        public static GameObject TestResPrefab { get => ResSystem.LoadAsset<GameObject>(nameof(TestResPrefab)); }  
        public static GameObject TestResPrefab_GameObject(Transform parent = null,string keyName=null,bool autoRelease = true)
        {
            return ResSystem.InstantiateGameObject("TestResPrefab", parent, keyName,autoRelease);
        }
    } 
    public static class Test
    {
 
        public static Texture2D TESTIMAGE { get => ResSystem.LoadAsset<Texture2D>(nameof(TESTIMAGE)); }  
        public static Sprite TESTIMAGE_TESTIMAGE_0 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_0]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_1 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_1]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_2 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_2]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_3 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_3]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_4 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_4]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_5 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_5]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_6 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_6]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_7 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_7]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_8 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_8]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_9 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_9]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_10 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_10]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_11 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_11]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_12 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_12]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_13 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_13]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_14 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_14]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_15 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_15]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_16 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_16]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_17 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_17]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_18 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_18]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_19 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_19]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_20 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_20]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_21 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_21]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_22 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_22]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_23 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_23]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_24 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_24]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_25 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_25]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_26 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_26]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_27 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_27]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_28 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_28]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_29 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_29]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_30 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_30]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_31 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_31]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_32 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_32]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_33 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_33]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_34 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_34]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_35 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_35]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_36 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_36]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_37 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_37]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_38 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_38]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_39 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_39]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_40 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_40]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_41 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_41]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_42 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_42]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_43 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_43]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_44 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_44]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_45 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_45]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_46 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_46]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_47 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_47]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_48 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_48]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_49 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_49]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_50 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_50]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_51 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_51]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_52 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_52]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_53 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_53]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_54 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_54]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_55 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_55]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_56 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_56]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_57 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_57]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_58 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_58]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_59 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_59]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_60 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_60]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_61 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_61]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_62 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_62]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_63 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_63]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_64 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_64]"); }  
        public static Sprite TESTIMAGE_TESTIMAGE_65 { get => ResSystem.LoadAsset<Sprite>("TESTIMAGE[TESTIMAGE_65]"); } 
        public static GameObject Cube { get => ResSystem.LoadAsset<GameObject>(nameof(Cube)); }  
        public static GameObject Cube_GameObject(Transform parent = null,string keyName=null,bool autoRelease = true)
        {
            return ResSystem.InstantiateGameObject("Cube", parent, keyName,autoRelease);
        }
    }
}