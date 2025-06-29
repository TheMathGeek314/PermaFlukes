using Modding;
using System.Reflection;
using UnityEngine;

namespace PermaFlukes {
    public class PermaFlukes: Mod {
        new public string GetName() => "PermaFlukes";
        public override string GetVersion() => "1.0.0.1";

        public override void Initialize() {
            On.SpellFluke.Burst += dontBurst;
            On.SpellFluke.OnEnable += addBurstComp;
            TransitionBurster.staticDefine();
        }

        private void dontBurst(On.SpellFluke.orig_Burst orig, SpellFluke self) {
            return;
        }

        private void addBurstComp(On.SpellFluke.orig_OnEnable orig, SpellFluke self) {
            orig(self);
            if(self.gameObject.TryGetComponent(out TransitionBurster tb))
                tb.sceneName = GameManager.instance.sceneName;
            else
                self.gameObject.AddComponent<TransitionBurster>();
        }
    }
    public class TransitionBurster: MonoBehaviour {
        public string sceneName = "";
        private MeshRenderer mr;
        private Rigidbody2D bd;
        private static FieldInfo meshRenderer, body;

        public TransitionBurster() {
            sceneName = GameManager.instance.sceneName;
        }

        public static void staticDefine() {
            meshRenderer = typeof(SpellFluke).GetField("meshRenderer", BindingFlags.NonPublic | BindingFlags.Instance);
            body = typeof(SpellFluke).GetField("body", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void Update() {
            if(GameManager.instance.sceneName != sceneName) {
                SpellFluke self = gameObject.GetComponent<SpellFluke>();
                if(mr == null)
                    mr = meshRenderer.GetValue(self) as MeshRenderer;
                if(bd == null)
                    bd = body.GetValue(self) as Rigidbody2D;
                if(mr)
                    mr.enabled = false;
                if(bd) {
                    bd.velocity = Vector2.zero;
                    bd.angularVelocity = 0f;
                    bd.isKinematic = true;
                }
                self.gameObject.Recycle();
            }
        }
    }
}