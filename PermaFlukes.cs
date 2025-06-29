using Modding;
using System.Reflection;
using UnityEngine;
namespace PermaFlukes {
    public class PermaFlukes: Mod {
        new public string GetName() => "PermaFlukes";
        public override string GetVersion() => "1.0.0.0";
        public override void Initialize() {
            On.SpellFluke.Burst += dontBurst;
            On.SpellFluke.OnEnable += addBurstComp;
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
        private FieldInfo meshRenderer, body;
        public TransitionBurster() {
            sceneName = GameManager.instance.sceneName;
            meshRenderer = typeof(SpellFluke).GetField("meshRenderer",
            BindingFlags.NonPublic | BindingFlags.Instance);
            body = typeof(SpellFluke).GetField("body",
            BindingFlags.NonPublic | BindingFlags.Instance);
        }
        private void Update() {
            if(GameManager.instance.sceneName != sceneName) {
                SpellFluke self = gameObject.GetComponent<SpellFluke>();
                MeshRenderer mr = meshRenderer.GetValue(self) as MeshRenderer;
                Rigidbody2D bd = body.GetValue(self) as Rigidbody2D;
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