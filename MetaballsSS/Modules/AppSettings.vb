Imports System
Imports System.Globalization
Imports Microsoft.Win32

''' <summary>
''' Central place for every user setting. Call LoadSettings() to read all
''' values from HKCU into the properties below, change the properties, then
''' call SaveSettings() to write them all back. The config dialog (/c) and the
''' actual screensaver (/s) both go through here so they agree on what to draw.
''' </summary>
Public Module AppSettings

   Private Const RegistryPath As String = "Software\Remus RIGO\MetaballsSS.VB-NET"

   ' Defaults / valid ranges
   Private Const DefaultBlobCount As Integer = 3
   Private Const MinBlobCount As Integer = 2
   Private Const MaxBlobCount As Integer = 3

   Private Const DefaultSpeed As Double = 1.0
   Private Const MinSpeed As Double = 0.25
   Private Const MaxSpeed As Double = 3.0

   Private Const DefaultRadiusScale As Double = 1.0
   Private Const MinRadiusScale As Double = 0.5
   Private Const MaxRadiusScale As Double = 2.0

   Private Const DefaultGlowIntensity As Double = 1.0
   Private Const MinGlowIntensity As Double = 0.25
   Private Const MaxGlowIntensity As Double = 3.0

   ' Current values (defaults until LoadSettings is called)
   Public Property PaletteKind As ColorPalette.PaletteKind = ColorPalette.PaletteKind.Blue
   Public Property BlobCount As Integer = DefaultBlobCount
   Public Property SpeedMultiplier As Double = DefaultSpeed
   Public Property RadiusMultiplier As Double = DefaultRadiusScale
   Public Property GlowIntensity As Double = DefaultGlowIntensity

   ''' <summary>Reads all settings from the registry; missing or invalid values fall back to defaults.</summary>
   Public Sub LoadSettings()
      PaletteKind = ColorPalette.PaletteKind.Blue
      BlobCount = DefaultBlobCount
      SpeedMultiplier = DefaultSpeed
      RadiusMultiplier = DefaultRadiusScale
      GlowIntensity = DefaultGlowIntensity

      Try
         Using key = Registry.CurrentUser.OpenSubKey(RegistryPath)
            If key Is Nothing Then Return

            Dim palette = TryCast(key.GetValue("Palette"), String)
            If String.Equals(palette, "Red", StringComparison.OrdinalIgnoreCase) Then
               PaletteKind = ColorPalette.PaletteKind.Red
            End If

            Dim count = key.GetValue("BlobCount")
            If count IsNot Nothing Then
               Dim n As Integer
               If Integer.TryParse(count.ToString(), n) AndAlso n >= MinBlobCount AndAlso n <= MaxBlobCount Then
                  BlobCount = n
               End If
            End If

            SpeedMultiplier = ReadDouble(key, "Speed", DefaultSpeed, MinSpeed, MaxSpeed)
            RadiusMultiplier = ReadDouble(key, "RadiusScale", DefaultRadiusScale, MinRadiusScale, MaxRadiusScale)
            GlowIntensity = ReadDouble(key, "GlowIntensity", DefaultGlowIntensity, MinGlowIntensity, MaxGlowIntensity)
         End Using
      Catch
         ' Registry unavailable - keep defaults.
      End Try
   End Sub

   ''' <summary>Writes all current settings to the registry.</summary>
   Public Sub SaveSettings()
      Try
         Using key = Registry.CurrentUser.CreateSubKey(RegistryPath)
            key.SetValue("Palette", PaletteKind.ToString())
            key.SetValue("BlobCount", BlobCount)
            key.SetValue("Speed", SpeedMultiplier.ToString(CultureInfo.InvariantCulture))
            key.SetValue("RadiusScale", RadiusMultiplier.ToString(CultureInfo.InvariantCulture))
            key.SetValue("GlowIntensity", GlowIntensity.ToString(CultureInfo.InvariantCulture))
         End Using
      Catch
         ' Registry unavailable - settings just won't persist.
      End Try
   End Sub

   Private Function ReadDouble(key As RegistryKey, name As String, defaultValue As Double, minValue As Double, maxValue As Double) As Double
      Dim v = TryCast(key.GetValue(name), String)
      Dim parsed As Double
      If v IsNot Nothing AndAlso Double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, parsed) Then
         If parsed >= minValue AndAlso parsed <= maxValue Then Return parsed
      End If
      Return defaultValue
   End Function

End Module
