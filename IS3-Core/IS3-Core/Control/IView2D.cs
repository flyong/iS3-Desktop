using iS3.Core.Geometry;
using iS3.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{

    public interface IView2D:IView
    {
        EngineeringMap eMap { get; }
        IEnumerable<IGraphicsLayer> layers { get; }
        ISpatialReference spatialReference { get; }
        // Summary:
        //     Each view has a drawing layer
        IGraphicsLayer drawingLayer { get; }

        // Summary:
        //     Screen point to map point conversions
        //
        IMapPoint screenToLocation(System.Windows.Point screenPoint);
        System.Windows.Point locationToScreen(IMapPoint mapPoint);

        // Summary:
        //     Controls whether graphics are selectable on a layer
        // Remarks:
        //     If layerID is '_ALL', then all layers are selectable.
        void addSeletableLayer(string layerID);
        // Summary:
        //     Controls whether graphics are selectable on a layer
        // Remarks:
        //     If layerID is '_ALL', then all layers are unselectable.
        void removeSelectableLayer(string layerID);

        // View control
        //
        void zoomTo(IGeometry geom);

        // Layers
        //
        void addLayer(IGraphicsLayer layer);
        IGraphicsLayer getLayer(string layerID);
        IGraphicsLayer removeLayer(string layerID);

        // Summary:
        //     Add a local tiled layer (.TPK file)
        // Parameter:
        //     filePath: full file name to .TPK
        //     id: layer id
        void addLocalTiledLayer(string filePath, string layerID);
        // Summary:
        //     Add a layer from local geodatabase dynamically.
        // Parameters:
        //     eLayer: specify the layer name and display options
        //     dbFile: geodatabase file name
        //     start: start index of the feature in the layer,
        //            default to zero.
        //     maxFeatures: maximum features allowed in loading the layer,
        //            default to zero (no limit).
        // Return value:
        //     a graphics layer 
        //
        Task<IGraphicsLayer> addGdbLayer(LayerDef layerDef,
            string dbFile, int start = 0, int maxFeatures = 0);
        // Summary:
        //     Add a layer from a shape file dynamically.
        // Parameters:
        //     eLayer: specify the layer name and display options
        //     shpFile: shape file name
        //     start: start index of the feature in the layer,
        //            default to zero.
        //     maxFeatures: maximum features allowed in loading the layer,
        //            default to zero (no limit).
        // Return value:
        //     a graphics layer 
        //
        Task<IGraphicsLayer> addShpLayer(LayerDef layerDef,
            string shpFile, int start = 0, int maxFeatures = 0);

        // Load predefined layers
        //
        Task loadPredefinedLayers();

        // Summary:
        //     The drawing graphics added/removed event trigger
        event EventHandler<DrawingGraphicsChangedEventArgs>
            drawingGraphicsChangedTrigger;
    }
}
