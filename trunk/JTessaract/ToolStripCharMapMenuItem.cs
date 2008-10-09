using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JTessaract
{
    public delegate void CharacterMapClickEventDelegate(string charactor);

    public class ToolStripCharMapMenuItem : ToolStripMenuItem
    {
        private string character;

        public ToolStripCharMapMenuItem(string character)
        {
            this.character = character;          
        }

        private ToolStripCharMapMenuItem()
        {

        }

        public event CharacterMapClickEventDelegate CharacterMapClickEvent = null;

        protected override void OnClick(EventArgs e)
        {
            if (CharacterMapClickEvent != null)
            {
                CharacterMapClickEvent(character);
            }

            base.OnClick(e);
        }
    }
}
