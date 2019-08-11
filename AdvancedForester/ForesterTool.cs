

using NetworkUI;
using NetworkUI.Items;
using Pipliz;
using System.Collections.Generic;

namespace AdvancedForester
{
    [ModLoader.ModManager]
    public static class ForesterTool
    {
        public static Dictionary<NetworkID, int> last_forester = new Dictionary<NetworkID, int>();

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerClicked, "Khanx.AdvancedForester.OnPlayerClicked")]
        public static void OnPlayerClicked(Players.Player player, Shared.PlayerClickedData playerClickedData)
        {
            if (player == null || playerClickedData.TypeSelected != ItemTypes.IndexLookup.GetIndex("Khanx.ForesterTool"))
                return;

            switch (playerClickedData.ClickType)
            {
                case Shared.PlayerClickedData.EClickType.Left:
                    Left_Click(player);
                    break;

                case Shared.PlayerClickedData.EClickType.Right:
                    Right_Click(player);
                    break;
            };
        }

        private static void Right_Click(Players.Player player)
        {
            int forester = last_forester.GetValueOrDefault(player.ID, 0);

            AreaJobTracker.CommandToolTypeData data = new AreaJobTracker.CommandToolTypeData();
            
            switch(forester)
            {
                default:
                case 0:
                    data.LocaleEntry = "popup.tooljob.cherryforester";
                    data.AreaType = "Khanx.CherryForester";
                break;
                case 1:
                    data.LocaleEntry = "popup.tooljob.autummforester";
                    data.AreaType = "Khanx.AutummForester";
                    break;
                case 2:
                    data.LocaleEntry = "popup.tooljob.taigaforester";
                    data.AreaType = "Khanx.TaigaForester";
                    break;
                case 3:
                    data.LocaleEntry = "popup.tooljob.temperateforester";
                    data.AreaType = "Khanx.TemperateForester";
                    break;
                case 4:
                    data.LocaleEntry = "popup.tooljob.oliveforester";
                    data.AreaType = "Khanx.OliveForester";
                    break;
            }

            data.Minimum3DBlockCount = 36;
            data.Maximum3DBlockCount = 100;
            data.Minimum2DBlockCount = 36;
            data.Maximum2DBlockCount = 100;
            data.MinimumHeight = 1;
            data.MaximumHeight = 3;
            data.OneAreaOnly = true;

            AreaJobTracker.StartCommandToolSelection(player, data);
        }

        private static void Left_Click(Players.Player player)
        {
            NetworkMenu menu = new NetworkMenu();
            menu.Identifier = "Avanced Forester";

            menu.LocalStorage.SetAs("header", Localization.GetSentence(player.LastKnownLocale, "popup.tooljob.advancedforesterheader"));

            ButtonCallback cherryButton = new ButtonCallback("Khanx.AdvancedForester.0", new LabelData(Localization.GetSentence(player.LastKnownLocale, "popup.tooljob.cherryforester"), UnityEngine.Color.black, UnityEngine.TextAnchor.MiddleCenter), -1, 25, ButtonCallback.EOnClickActions.ClosePopup);
            ButtonCallback autummButton = new ButtonCallback("Khanx.AdvancedForester.1", new LabelData(Localization.GetSentence(player.LastKnownLocale, "popup.tooljob.autummforester"), UnityEngine.Color.black, UnityEngine.TextAnchor.MiddleCenter), -1, 25, ButtonCallback.EOnClickActions.ClosePopup);
            ButtonCallback taigaButton = new ButtonCallback("Khanx.AdvancedForester.2", new LabelData(Localization.GetSentence(player.LastKnownLocale, "popup.tooljob.taigaforester"), UnityEngine.Color.black, UnityEngine.TextAnchor.MiddleCenter), -1, 25, ButtonCallback.EOnClickActions.ClosePopup);
            ButtonCallback temperateButton = new ButtonCallback("Khanx.AdvancedForester.3", new LabelData(Localization.GetSentence(player.LastKnownLocale, "popup.tooljob.temperateforester"), UnityEngine.Color.black, UnityEngine.TextAnchor.MiddleCenter), -1, 25, ButtonCallback.EOnClickActions.ClosePopup);
            ButtonCallback oliveButton = new ButtonCallback("Khanx.AdvancedForester.4", new LabelData(Localization.GetSentence(player.LastKnownLocale, "popup.tooljob.oliveforester"), UnityEngine.Color.black, UnityEngine.TextAnchor.MiddleCenter), -1, 25, ButtonCallback.EOnClickActions.ClosePopup);

            menu.Items.Add(cherryButton);
            menu.Items.Add(autummButton);
            menu.Items.Add(taigaButton);
            menu.Items.Add(temperateButton);
            menu.Items.Add(oliveButton);

            NetworkMenuManager.SendServerPopup(player, menu);
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerPushedNetworkUIButton, "Khanx.Compass.OnPlayerPushedNetworkUIButton")]
        public static void OnPlayerPushedNetworkUIButton(ButtonPressCallbackData data)
        {
            if (!data.ButtonIdentifier.StartsWith("Khanx.AdvancedForester."))
                return;

            Log.Write("Forester ID:" + data.ButtonIdentifier.Substring(data.ButtonIdentifier.LastIndexOf(".") + 1));

            int foresterID = int.Parse(data.ButtonIdentifier.Substring(data.ButtonIdentifier.LastIndexOf(".")+1));

            if (last_forester.ContainsKey(data.Player.ID))
                last_forester.Remove(data.Player.ID);

            last_forester.Add(data.Player.ID, foresterID);
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerDisconnected, "Khanx.AdvancedForester.OnPlayerDisconnected")]
        public static void OnPlayerDisconnected(Players.Player player)
        {
            if (last_forester.ContainsKey(player.ID))
                last_forester.Remove(player.ID);
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnSendAreaHighlights, "Khanx.AdvancedWand.ShowArea")]
        public static void OnSendAreaHighlights(Players.Player player, List<AreaJobTracker.AreaHighlight> list, List<ushort> showWhileHoldingTypes)
        {
            if (null != player)
            {
                showWhileHoldingTypes.Add(ItemTypes.IndexLookup.GetIndex("Khanx.ForesterTool"));
            }
        }
    }
}
