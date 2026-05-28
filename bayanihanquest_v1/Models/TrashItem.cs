using System.Windows.Forms;

namespace Windows_form_game_V1._0.Models
{
    public class TrashItem
    {
        public PictureBox Sprite { get; private set; }
        public bool IsCollected { get; private set; }

        public TrashItem(PictureBox sprite)
        {
            Sprite = sprite;
        }

        public void Collect()
        {
            IsCollected = true;
            Sprite.Visible = false;
        }
    }
}
