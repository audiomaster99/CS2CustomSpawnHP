namespace CustomHealth;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text.Json.Serialization;

public class Config : BasePluginConfig
{
    [JsonPropertyName("player-spawn-hp")]
    public int SetPlayerHealth { get; set; } = 35;
}
public partial class CustomHealth : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "CustomSpawnHealth";
    public override string ModuleAuthor => "audio_brutalci";
    public override string ModuleVersion => "0.0.1";

    public required Config Config { get; set; }
    public void OnConfigParsed(Config config)
    {
        Config = config;
    }
    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        if (player == null
        || player.PlayerPawn == null
        || !player.IsValid
        || !player.PlayerPawn.IsValid
        || player.Connected != PlayerConnectedState.PlayerConnected
        || player.PlayerPawn.Value?.LifeState != (byte)LifeState_t.LIFE_ALIVE)
        {
            return HookResult.Continue;
        }
        if (player.PlayerPawn.Value.Health > Config.SetPlayerHealth)
        {
            Server.NextFrame(() =>
            {
                player.PlayerPawn.Value.Health = Config.SetPlayerHealth;
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
            });
        }
        return HookResult.Continue;
    }
}
