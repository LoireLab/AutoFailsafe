using Jobs;
using ModLoaderInterfaces;
using NetworkUI;
using NetworkUI.AreaJobs;
using NetworkUI.Items;

namespace AdvancedForester
{
    public class ForesterInterface : IOnConstructCommandTool, IOnPlayerPushedNetworkUIButton, IAfterWorldLoad
    {
        public void AfterWorldLoad()
        {
            CommandToolManager.MenuTooltips.Add("Khanx.AdvancedForester", ("popup.tooljob.advancedforestera", "popup.tooljob.advancedforesterb"));

            CommandToolManager.MenuTooltips.Add("Khanx.CherryForest", ("popup.tooljob.cherryforestera", "popup.tooljob.cherryforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.CherryForest",
                AreaToolDescriptionSettings.NewForest("Cherry Forester", "Khanx.CherryForester", "pipliz.forester"));

            CommandToolManager.MenuTooltips.Add("Khanx.TaigaForest", ("popup.tooljob.taigaforestera", "popup.tooljob.taigaforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.TaigaForest",
                AreaToolDescriptionSettings.NewForest("Taiga Forester", "Khanx.TaigaForester", "pipliz.forester"));

            CommandToolManager.MenuTooltips.Add("Khanx.OliveForest", ("popup.tooljob.oliveforestera", "popup.tooljob.oliveforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.OliveForest",
                AreaToolDescriptionSettings.NewForest("Olive Forester", "Khanx.OliveForester", "pipliz.forester"));


            CommandToolManager.MenuTooltips.Add("Khanx.AutummRedForester", ("popup.tooljob.autumredforestera", "popup.tooljob.autumredforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.AutummRedForester",
                AreaToolDescriptionSettings.NewForest("popup.tooljob.autumredforester", "Khanx.AutummRedForester", "pipliz.forester"));

            CommandToolManager.MenuTooltips.Add("Khanx.AutummOrangeForester", ("popup.tooljob.autummorangeforestera", "popup.tooljob.autummorangeforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.AutummOrangeForester",
                AreaToolDescriptionSettings.NewForest("popup.tooljob.autummorangeforester", "Khanx.AutummOrangeForester", "pipliz.forester"));

            CommandToolManager.MenuTooltips.Add("Khanx.AutummYellowForester", ("popup.tooljob.autumyellowforestera", "popup.tooljob.autumyellowforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.AutummYellowForester",
                AreaToolDescriptionSettings.NewForest("popup.tooljob.autumyellowforester", "Khanx.AutummYellowForester", "pipliz.forester"));


            CommandToolManager.MenuTooltips.Add("Khanx.DarkTemperateForester", ("popup.tooljob.darktemperateforestera", "popup.tooljob.darktemperateforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.DarkTemperateForester",
                AreaToolDescriptionSettings.NewForest("popup.tooljob.darktemperateforester", "Khanx.DarkTemperateForester", "pipliz.forester"));

            CommandToolManager.MenuTooltips.Add("Khanx.TemperateForester", ("popup.tooljob.temperateforestera", "popup.tooljob.temperateforestera"));
            CommandToolManager.AreaDescriptions.Add("Khanx.TemperateForester",
                AreaToolDescriptionSettings.NewForest("popup.tooljob.temperateforester", "Khanx.TemperateForester", "pipliz.forester"));

            CommandToolManager.MenuTooltips.Add("Khanx.LightTemperateForester", ("popup.tooljob.lighttemperateforestera", "popup.tooljob.lighttemperateforesterb"));
            CommandToolManager.AreaDescriptions.Add("Khanx.LightTemperateForester",
                AreaToolDescriptionSettings.NewForest("popup.tooljob.lighttemperateforester", "Khanx.LightTemperateForester", "pipliz.forester"));
        }

        public void OnConstructCommandTool(Players.Player player, NetworkMenu networkMenu, string menuName)
        {
            if (!menuName.Equals(CommandToolManager.Menus.MAIN_MENU))
                return;

            HorizontalRow horizontalRow = (HorizontalRow)networkMenu.Items[4];
            horizontalRow.Items[1] = (CommandToolManager.GetButtonMenu(player, "Khanx.AdvancedForester", "popup.tooljob.advancedforester"), horizontalRow.Items[1].Item2);
        }

        public void OnPlayerPushedNetworkUIButton(ButtonPressCallbackData data)
        {
            if (!CommandToolManager.TryStartCommandToolSelection(data.Player, data.ButtonIdentifier))
            {
                switch (data.ButtonIdentifier)
                {
                    case "Khanx.AdvancedForester":
                        NetworkMenu networkMenu = CommandToolManager.GenerateMenuBase(data.Player, showBackButton: true);
                        CommandToolManager.GenerateThreeColumnCenteredRow(networkMenu, new EmptySpace(), CommandToolManager.GetButtonTool(data.Player, "pipliz.temperateforest", "popup.tooljob.forester", 200), new EmptySpace());
                        networkMenu.Items.Add(new EmptySpace(10));
                        
                        CommandToolManager.GenerateThreeColumnCenteredRow(networkMenu, CommandToolManager.GetButtonTool(data.Player, "Khanx.CherryForest", "popup.tooljob.cherryforester", 200), CommandToolManager.GetButtonTool(data.Player, "Khanx.TaigaForest", "popup.tooljob.taigaforester", 200), CommandToolManager.GetButtonTool(data.Player, "Khanx.OliveForest", "popup.tooljob.oliveforester", 200));
                        networkMenu.Items.Add(new EmptySpace(10));

                        CommandToolManager.GenerateThreeColumnCenteredRow(networkMenu, CommandToolManager.GetButtonTool(data.Player, "Khanx.AutummRedForester", "popup.tooljob.autumredforester", 200), CommandToolManager.GetButtonTool(data.Player, "Khanx.AutummOrangeForester", "popup.tooljob.autummorangeforester", 200), CommandToolManager.GetButtonTool(data.Player, "Khanx.AutummYellowForester", "popup.tooljob.autumyellowforester", 200));
                        networkMenu.Items.Add(new EmptySpace(10));

                        CommandToolManager.GenerateThreeColumnCenteredRow(networkMenu, CommandToolManager.GetButtonTool(data.Player, "Khanx.DarkTemperateForester", "popup.tooljob.darktemperateforester", 200), CommandToolManager.GetButtonTool(data.Player, "Khanx.TemperateForester", "popup.tooljob.temperateforester", 200), CommandToolManager.GetButtonTool(data.Player, "Khanx.LightTemperateForester", "popup.tooljob.lighttemperateforester", 200));

                        NetworkMenuManager.SendServerPopup(data.Player, networkMenu);
                        break;
                }
            }
        }
    }
}
