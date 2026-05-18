// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace CodaGame.Editor
{
    /// <summary>
    /// Editor-only helpers for programmatic Addressables registration.
    /// Used by <see cref="ConfigImporterWindow"/> to auto-register imported configs.
    /// </summary>
    public static class AddressableEditorUtility
    {
        /// <summary>
        /// Register an asset into the Addressables system under the given group and address.
        /// Idempotent: if the asset is already in another group, it is moved; if it is already in the
        /// target group, only its address is updated. Creates the group if it does not exist (copying
        /// the schemas from the project's default group).
        /// </summary>
        /// <param name="_asset">The asset to register.</param>
        /// <param name="_groupName">Name of the Addressables group to place the asset into.</param>
        /// <param name="_address">The address to assign to the asset entry.</param>
        /// <returns>True on success, false if Addressables settings are not initialized or the asset has no path.</returns>
        public static bool RegisterAddressable(Object _asset, string _groupName, string _address)
        {
            if (_asset == null)
            {
                Console.LogError(SystemNames.Config, "RegisterAddressable: asset is null.");
                return false;
            }

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Console.LogError(SystemNames.Config, "RegisterAddressable: Addressables settings are not initialized. " +
                                                     "Open Window → Asset Management → Addressables → Groups to create them.");
                return false;
            }

            string assetPath = AssetDatabase.GetAssetPath(_asset);
            if (string.IsNullOrEmpty(assetPath))
            {
                Console.LogError(SystemNames.Config, $"RegisterAddressable: asset '{_asset.name}' has no path on disk.");
                return false;
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
            {
                Console.LogError(SystemNames.Config, $"RegisterAddressable: could not resolve GUID for path '{assetPath}'.");
                return false;
            }

            AddressableAssetGroup group = settings.FindGroup(_groupName);
            if (group == null)
            {
                AddressableAssetGroup defaultGroup = settings.DefaultGroup;
                group = settings.CreateGroup(
                    _groupName,
                    setAsDefaultGroup: false,
                    readOnly: false,
                    postEvent: true,
                    schemasToCopy: defaultGroup != null ? defaultGroup.Schemas : null);
                Console.LogSystem(SystemNames.Config, $"Addressables: created group '{_groupName}'.");
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            if (entry == null)
            {
                Console.LogError(SystemNames.Config, $"RegisterAddressable: failed to create or move entry for '{assetPath}'.");
                return false;
            }

            entry.SetAddress(_address);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
            return true;
        }
    }
}
