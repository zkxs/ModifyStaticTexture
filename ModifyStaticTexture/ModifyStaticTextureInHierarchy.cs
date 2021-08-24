using BaseX;
using CodeX;
using FrooxEngine;
using FrooxEngine.UIX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifyStaticTexture
{
    [Category("Plugin/runtime")]
    public class ModifyStaticTextureInHierarchy : Component, ICustomInspector
    {
        public readonly SyncRef<Slot> TargetHierarchy;
        public readonly Sync<bool> IncludeDisabled;
        public readonly Sync<int2> MinimumSize;
        public readonly Sync<int2?> MaximumSize;

        public readonly Sync<TextureFilterMode?> FilterMode;
        public readonly Sync<int?> AnisotropicLevel;
        public readonly Sync<bool?> Uncompressed;
        public readonly Sync<bool?> DirectLoad;
        public readonly Sync<bool?> ForceExactVariant;
        public readonly Sync<TextureCompression?> PreferredFormat;
        public readonly Sync<float?> MipMapBias;
        public readonly Sync<bool?> CrunchCompressed;
        public readonly Sync<bool?> MipMaps;

        protected override void OnAttach()
        {
            TargetHierarchy.Target = Slot;
        }

        public void BuildInspectorUI(UIBuilder ui)
        {
            WorkerInspector.BuildInspectorUI(this, ui);
            UIBuilder uiBuilder = ui;
            ui.Button("Process StaticTexture2Ds", new ButtonEventHandler(ActivateAction));
        }

        private void ActivateAction(IButton button, ButtonEventData eventData)
        {
            Slot slot = TargetHierarchy.Target;
            if (slot == null)
            {
                button.LabelText = "Process StaticTexture2Ds (TargetHierarchy must be non-null)";
            }
            else
            {
                int updateCount = Process(slot);
                button.LabelText = $"Process StaticTexture2Ds ({updateCount} updated)";
            }
        }

        private int Process(Slot hierarchy)
        {
            int updateCount = 0;
            List<StaticTexture2D> components = hierarchy.GetComponentsInChildren<StaticTexture2D>(CheckSize, !IncludeDisabled.Value);
            foreach (StaticTexture2D component in components)
            {
                if (Process(component))
                {
                    updateCount += 1;
                }
            }
            return updateCount;
        }

        private bool Process(StaticTexture2D component)
        {
            bool updated = false;

            if (FilterMode.Value is TextureFilterMode filterMode && filterMode != component.FilterMode.Value)
            {
                updated = true;
                component.FilterMode.Value = filterMode;
            }

            if (AnisotropicLevel.Value is int anisotropicLevel && anisotropicLevel != component.AnisotropicLevel.Value)
            {
                updated = true;
                component.AnisotropicLevel.Value = anisotropicLevel;
            }

            if (Uncompressed.Value is bool uncompressed && uncompressed != component.Uncompressed.Value)
            {
                updated = true;
                component.Uncompressed.Value = uncompressed;
            }

            if (DirectLoad.Value is bool directLoad && directLoad != component.DirectLoad.Value)
            {
                updated = true;
                component.DirectLoad.Value = directLoad;
            }

            if (ForceExactVariant.Value is bool forceExactVariant && forceExactVariant != component.ForceExactVariant.Value)
            {
                updated = true;
                component.ForceExactVariant.Value = forceExactVariant;
            }

            if (PreferredFormat.Value is TextureCompression preferredFormat && preferredFormat != component.PreferredFormat.Value)
            {
                updated = true;
                component.PreferredFormat.Value = preferredFormat;
            }

            if (MipMapBias.Value is float mipMapBias && mipMapBias != component.MipMapBias.Value)
            {
                updated = true;
                component.MipMapBias.Value = mipMapBias;
            }

            if (CrunchCompressed.Value is bool crunchCompressed && crunchCompressed != component.CrunchCompressed.Value)
            {
                updated = true;
                component.CrunchCompressed.Value = crunchCompressed;
            }

            if (MipMaps.Value is bool mipMaps && mipMaps != component.MipMaps.Value)
            {
                updated = true;
                component.MipMaps.Value = mipMaps;
            }

            return updated;
        }

        private bool CheckSize(StaticTexture2D component)
        {
            if (component.Asset?.Size is int2 size && size.x > 0 && size.y > 0 && size.x >= MinimumSize.Value.x && size.y >= MinimumSize.Value.y)
            {
                // min size check passed
                if (MaximumSize.Value is int2 max)
                {
                    // there was a max size
                    return size.x <= max.x && size.y <= max.y;
                }
                else
                {
                    // there was no max size
                    return true;
                }
            }
            else
            {
                // min size check failed
                return false;
            }
        }
    }
}
