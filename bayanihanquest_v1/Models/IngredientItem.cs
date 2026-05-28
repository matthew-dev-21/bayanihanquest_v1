using System.Windows.Forms;

namespace Windows_form_game_V1._0.Models
{
    public class IngredientItem
    {
        public PictureBox Sprite { get; private set; }
        public string IngredientType { get; private set; }
        public bool IsCollected { get; private set; }

        public IngredientItem(PictureBox sprite, string ingredientType)
        {
            Sprite = sprite;
            IngredientType = ingredientType;
        }

        public void Collect()
        {
            IsCollected = true;
            Sprite.Visible = false;
        }
    }
}
