using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace CLogic.Utils.UI
{
    public class GraphicsFader : MonoBehaviour
    {
        /// <summary>
        /// The graphics on which the fading shall be done
        /// </summary>
        public Graphic[] graphics;
        
        /// <summary>
        /// Sets the alpha of all graphics to the given value
        /// </summary>
        /// <param name="alpha">The alpha to be set</param>
        public void SetAlpha(float alpha)
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color = graphic.color.WithAlpha(alpha);
            }
        }
        
        /// <summary>
        /// Sets the color of all graphics to the given value
        /// </summary>
        /// <param name="color">The color to be set</param>
        public void SetColor(Color color)
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color = color;
            }
        }
        
        /// <summary>
        /// Multiplies the color of each graphic with the given value
        /// </summary>
        /// <param name="color">The color to multiply with</param>
        public void MultiplyColor(Color color)
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.color *= color;
            }
        }
        
        /// <summary>
        /// Sets the alpha of the graphics of multiple faders
        /// </summary>
        /// <param name="alpha">The alpha to be set</param>
        /// <param name="faders">The faders whose graphics are affected</param>
        public static void SetAlpha(float alpha, params GraphicsFader[] faders)
        {
            foreach (GraphicsFader fader in faders)
            {
                fader.SetAlpha(alpha);
            }
        }
        
        /// <summary>
        /// Sets the color of the graphics of multiple faders
        /// </summary>
        /// <param name="color">The color to be set</param>
        /// <param name="faders">The faders whose graphics are affected</param>
        public static void SetColor(Color color, params GraphicsFader[] faders)
        {
            foreach (GraphicsFader fader in faders)
            {
                fader.SetColor(color);
            }
        }
        
        /// <summary>
        /// Multiplies the color of each fader's graphic with the given value
        /// </summary>
        /// <param name="color">The color to be set</param>
        /// <param name="faders">The faders whose graphics are affected</param>
        public static void MultiplyColor(Color color, params GraphicsFader[] faders)
        {
            foreach (GraphicsFader fader in faders)
            {
                fader.MultiplyColor(color);
            }
        }
        
        /// <summary>
        /// Automatically add any graphics in the current game object
        /// </summary>
        public void Reset()
        {
            Graphic[] g = GetComponents<Graphic>();
            
            if (g != null)
                graphics = g.ToArray();
        }
    }
}
