using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
//using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace StaticLetters
{
    [StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        public static Harmony harmonyInstance;


        static HarmonyPatches()
        {
            harmonyInstance = new Harmony("rimworld.rwmods.StaticLetters");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

    }


    [HarmonyPatch(typeof(Letter))]
    [HarmonyPatch("DrawButtonAt")]
    internal static class DrawStaticButton
    {

#if OLD_METHOD
        public static bool Prefix(ref Letter __instance, float topY)
        {

            float num1 = (float)((double)UI.screenWidth - 38.0 - 12.0);
            Rect rect1 = new Rect(num1, topY, 38f, 30f);
            Rect rect2 = new Rect(rect1);
            float num2 = Time.time - __instance.arrivalTime;
            Color color = __instance.def.color;
            if ((double)num2 < 1.0)
            {
                rect2.y -= (float)((1.0 - (double)num2) * 200.0);
                color.a = num2 / 1f;
            }
            /*
            if (!Mouse.IsOver(rect1) && __instance.def.bounce && (double)num2 > 15.0 && (double)num2 % 5.0 < 1.0)
            {
                double num3 = (double)UI.screenWidth * 0.0599999986588955;
                float num4 = (float)(2.0 * ((double)num2 % 1.0) - 1.0);
                double num5 = 1.0 - (double)num4 * (double)num4;
                float num6 = (float)(num3 * num5);
                rect2.x -= num6;
            }*/

            if (Event.current.type == EventType.Repaint)
            {
                if ((double)__instance.def.flashInterval > 0.0)
                {
                    float time = Time.time - (__instance.arrivalTime + 1f);
                    if ((double)time > 0.0 && (double)time % (double)__instance.def.flashInterval < 1.0)
                        GenUI.DrawFlash(num1, topY, (float)UI.screenWidth * 0.6f, Pulser.PulseBrightness(1f, 1f, time) * 0.55f, __instance.def.flashColor);
                }
                GUI.color = color;
                Widgets.DrawShadowAround(rect2);
                GUI.DrawTexture(rect2, (Texture)__instance.def.Icon);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperRight;
                string str = (string)__instance.label;
                Vector2 vector2_1 = Text.CalcSize(str);
                float x = vector2_1.x;
                float y = vector2_1.y;
                Vector2 vector2_2 = new Vector2(rect2.x + rect2.width / 2f, (float)((double)rect2.center.y - (double)y / 2.0 + 4.0));
                float num7 = vector2_2.x + x / 2f - (float)(UI.screenWidth - 2);
                if ((double)num7 > 0.0)
                    vector2_2.x -= num7;
                GUI.DrawTexture(new Rect((float)((double)vector2_2.x - (double)x / 2.0 - 6.0 - 1.0), vector2_2.y, x + 12f, 16f), (Texture)TexUI.GrayTextBG);
                GUI.color = new Color(1f, 1f, 1f, 0.75f);
                Widgets.Label(new Rect(vector2_2.x - x / 2f, vector2_2.y - 3f, x, 999f), str);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            if (__instance.CanDismissWithRightClick && Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.IsOver(rect1))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                Find.LetterStack.RemoveLetter(__instance);
                Event.current.Use();
            }
            if (Widgets.ButtonInvisible(rect2))
            {
                __instance.OpenLetter();
                Event.current.Use();
            }



            return false; // We dont want to run the old code
        }
#else
        // KISS...
        public static bool Prefix(ref Letter __instance, float topY)
        {
            __instance.def.bounce = false;          // Never bounce
            __instance.def.flashInterval = 0.0f;    // No flashing
            return true; // Use old code
        }
#endif
    }

}
