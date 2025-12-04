# Copilot Instructions for VRMap

## Project Overview
VRMap is a Unity-based application for mapping, geocoding, and visualizing points of interest (POIs) in a 3D environment. It integrates Cesium for Unity for geospatial visualization and uses OpenStreetMap's Nominatim API for geocoding addresses.

## Key Components
- **AppManager** (`Assets/Scripts/AppManager.cs`): Singleton entry point. Provides access to the geocoder (`GeoCodeAddress`).
- **GeoCodeAddress** (`Assets/Scripts/GeoCodeAddress.cs`): Handles address-to-lat/lon geocoding via Nominatim. Uses Unity coroutines for async web requests. Requires a valid contact email in the User-Agent header.
- **TourManager** (`Assets/Scripts/TourManager.cs`): Orchestrates POI spawning and geocoding. Listens for geocode success/error events. Instantiates POIs from a database.
- **POIDB** (`Assets/Scripts/POI/POIDB.cs`): ScriptableObject database of POIData objects.
- **POIData** (`Assets/Scripts/POI/POIData.cs`): Serializable POI data (name, lat, lon, height).
- **PointOfInterest** (`Assets/Scripts/POI/PointOfInterest.cs`): MonoBehaviour for POI visualization. Anchors to globe using CesiumGlobeAnchor.
- **HierarchyDecorator** (`Assets/Scripts/Tools/HirearchyDecorator.cs`): Unity Editor extension for hierarchy UI enhancements.

## Data Flow
- POIs are defined in `POIDB` assets and spawned by `TourManager` using the `PointOfInterest` prefab.
- Geocoding is performed by `GeoCodeAddress`, invoked via `AppManager.instance.geoCoder.Geocode(...)`.
- Geocode results update the tour's coordinates and can trigger POI placement.

## Developer Workflows
- **Build:** Use Unity Editor's build system. No custom build scripts detected.
- **Play/Debug:** Main entry is the Unity scene. Ensure `AppManager` is present in the scene for singleton access.
- **POI Management:** Add/edit POIs via the `POIDB` ScriptableObject in the Unity Inspector.
- **Geocoding:** Set the contact email in `GeoCodeAddress` to comply with Nominatim's usage policy.
- **Editor Extensions:** HierarchyDecorator improves hierarchy UI; only active in the Unity Editor.

## Conventions & Patterns
- **Singletons:** `AppManager` uses a static `instance` pattern.
- **Events:** Geocoding uses `Action` callbacks for success/error.
- **ScriptableObjects:** Used for POI database (`POIDB`).
- **Prefabs:** POIs are instantiated from a prefab reference in `TourManager`.
- **Cesium Integration:** POIs use `CesiumGlobeAnchor` for geospatial placement.

## Integration Points
- **Cesium for Unity:** Required for globe anchoring and geospatial visualization.
- **OpenStreetMap Nominatim:** Used for geocoding; respect API rate limits and usage policy.

## Examples
- To geocode an address and spawn POIs:
  ```csharp
  AppManager.instance.geoCoder.Geocode(address, onSuccess, onError);
  ```
- To add a POI, update the `POIDB` asset in the Unity Inspector.

## References
- Scripts: `Assets/Scripts/`
- POI Data: `Assets/Scripts/POI/`
- Editor Tools: `Assets/Scripts/Tools/`

---
_If any section is unclear or missing, please provide feedback for further refinement._
