using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NetStone.Definitions.Model;
using NetStone.Definitions.Model.Character;
using NetStone.Definitions.Model.FreeCompany;
using Newtonsoft.Json;

namespace NetStone.Definitions;

public class XivApiDefinitionsContainer : DefinitionsContainer
{
    private const string DefinitionRepoBase = "https://raw.githubusercontent.com/xivapi/lodestone-css-selectors/main/";

    private readonly HttpClient client;

    public XivApiDefinitionsContainer()
    {
        this.client = new HttpClient
        {
            BaseAddress = new Uri(DefinitionRepoBase),
        };
    }

    public override async Task Reload()
    {
        this.Meta = await GetDefinition<MetaDefinition>("meta.json");

            this.Character = await GetDefinition<CharacterDefinition>("profile/character.json");
            this.ClassJob = await GetDefinition<CharacterClassJobDefinition>("profile/classjob.json");
            this.Gear = await GetDefinition<CharacterGearDefinition>("profile/gearset.json");
            PatchCharacterGearDefinition();

            this.Attributes = await GetDefinition<CharacterAttributesDefinition>("profile/attributes.json");
            this.Achievement = await GetDefinition<PagedDefinition>("profile/achievements.json");
            this.Mount = await GetDefinition<CharacterCollectableDefinition>("profile/mount.json");
            this.Minion = await GetDefinition<CharacterCollectableDefinition>("profile/minion.json");

        this.FreeCompany = await GetDefinition<FreeCompanyDefinition>("freecompany/freecompany.json");
        this.FreeCompanyFocus = await GetDefinition<FreeCompanyFocusDefinition>("freecompany/focus.json");
        this.FreeCompanyReputation = await GetDefinition<FreeCompanyReputationDefinition>("freecompany/reputation.json");

        this.FreeCompanyMembers = await GetDefinition<PagedDefinition>("freecompany/members.json");

        this.CharacterSearch = await GetDefinition<PagedDefinition>("search/character.json");
        this.FreeCompanySearch = await GetDefinition<PagedDefinition>("search/freecompany.json");
    }

    private async Task<T> GetDefinition<T>(string path) where T : IDefinition
    {
        var json = await this.client.GetStringAsync(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

        // Remove every character in materia selector after <ul> to make a search after css classes for materia possible.
        // This is needed, because ``ul:nth-child(7)`` doesn't work if the item has been dyed.
        private void PatchCharacterGearDefinition()
        {
            var oldSelector = "ul:nth-child(7)";
            var newSelector = "ul.db-tooltip__materia";

            PatchAllMateriaSlots(this.Gear.Mainhand);
            PatchAllMateriaSlots(this.Gear.Offhand);
            PatchAllMateriaSlots(this.Gear.Head);
            PatchAllMateriaSlots(this.Gear.Body);
            PatchAllMateriaSlots(this.Gear.Hands);
            PatchAllMateriaSlots(this.Gear.Legs);
            PatchAllMateriaSlots(this.Gear.Feet);
            PatchAllMateriaSlots(this.Gear.Earrings);
            PatchAllMateriaSlots(this.Gear.Necklace);
            PatchAllMateriaSlots(this.Gear.Bracelets);
            PatchAllMateriaSlots(this.Gear.Ring1);
            PatchAllMateriaSlots(this.Gear.Ring2);

            void PatchAllMateriaSlots(GearEntryDefinition gearEntryDefinition)
            {
                gearEntryDefinition.Materia1.Selector = gearEntryDefinition.Materia1.Selector.Replace(oldSelector, newSelector);
                gearEntryDefinition.Materia2.Selector = gearEntryDefinition.Materia2.Selector.Replace(oldSelector, newSelector);
                gearEntryDefinition.Materia3.Selector = gearEntryDefinition.Materia3.Selector.Replace(oldSelector, newSelector);
                gearEntryDefinition.Materia4.Selector = gearEntryDefinition.Materia4.Selector.Replace(oldSelector, newSelector);
                gearEntryDefinition.Materia5.Selector = gearEntryDefinition.Materia5.Selector.Replace(oldSelector, newSelector);
            }
        }

        public override void Dispose()
        {
            this.client.Dispose();
        }
    }
}