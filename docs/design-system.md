# Design system

The gallery has two jobs that pull in different directions: it should look polished and modern, and it should prove
that .NET MAUI renders **real native controls**. The design system resolves this with one rule.

## Branded chrome, native stage

- **Chrome** is everything that belongs to the gallery: the sidebar, tabs, page headers, cards, the options pane
  and the code viewer. Chrome uses the tokens and components below.
- **The stage** is the dotted canvas where a sample renders. Samples use **platform defaults**: no app styles reach
  them, because the design system defines **no implicit styles** for core controls. Every chrome style is keyed.
- The stage toolbar makes the point explicit: a badge shows the native type the control became (read from
  `Handler.PlatformView` at runtime), **Default / Styled** applies an example app-wide style to just that sample (the control stays native either way), and
  **RTL** mirrors it.

## Tokens

Tokens live in `src/MauiGallery/DesignSystem/Tokens`. Colors have the same keys in `LightTheme.xaml` and
`DarkTheme.xaml`; `ThemeService` swaps the dictionaries when the theme changes, so chrome references colors with
`{DynamicResource ...}`. Sizes and type styles do not change with the theme and use `{StaticResource ...}`.

### Color

| Token | Light | Dark | Use |
| --- | --- | --- | --- |
| `ColorBackground` | `#F6F5FA` | `#0E0C13` | Page background |
| `ColorSurface` | `#FFFFFF` | `#17151E` | Cards, inputs |
| `ColorSurfaceAlt` | `#F0EEF6` | `#1E1B27` | Subtle panels, hover |
| `ColorSidebar` | `#F1EFF7` | `#121018` | Navigation pane |
| `ColorOutline` | `#E4E1EC` | `#2B2736` | Borders, dividers |
| `ColorTextPrimary` | `#16131D` | `#F4F2F9` | Body text |
| `ColorTextSecondary` | `#57536A` | `#B4AFC4` | Supporting text |
| `ColorAccent` | `#512BD4` | `#9B87F5` | .NET purple: primary actions, selection |
| `ColorAccentText` | `#512BD4` | `#B3A3FF` | Accent-colored text on surfaces |
| `ColorStage` | `#FBFBFD` | `#121118` | Sample stage: deliberately neutral |

Text tokens meet WCAG AA (4.5:1) on their surfaces. The purple-to-blue `BrushHero` gradient is reserved for hero
areas and category tiles. Syntax colors follow GitHub's Primer palette. The Color page in the app shows every token
live in the current theme.

### Typography

Inter for UI, Cascadia Code for code. Keyed `Label` styles:

| Style | Font | Size | Notes |
| --- | --- | --- | --- |
| `TextDisplay` | Inter Bold | 40 | Hero headlines only; heading level 1 |
| `TextTitle1` | Inter SemiBold | 28 | Page titles; heading level 1 |
| `TextTitle2` | Inter SemiBold | 22 | Heading level 2 |
| `TextTitle3` | Inter SemiBold | 18 | Section and example titles; heading level 3 |
| `TextBody` | Inter | 15 | Default text |
| `TextBodySecondary` | Inter | 15 | Descriptions |
| `TextBodyStrong` | Inter SemiBold | 15 | Card titles, emphasis |
| `TextCaption` | Inter | 12 | Metadata |
| `TextOverline` | Inter SemiBold | 11, caps | Section labels |
| `TextCode` | Cascadia Code | 13 | Code |

All text scales with the system text size.

### Spacing, shape and elevation

- **Spacing** on a 4-point grid: `Space4` … `Space64`. Page padding grows with the window (16, 32, 48).
- **Shape**: `ShapeInput` (6), `ShapeCard` (12), `ShapeHero` (20), `ShapePill`.
- **Elevation**: a soft shadow in light mode (`ShadowCard`); in dark mode outlines carry depth and shadows are off.

### Icons

[Fluent UI System Icons](https://github.com/microsoft/fluentui-system-icons) through `IconFont.Maui.FluentIcons`:
Regular for chrome, Filled for selected states (for example a favorite star). Use the `TextIcon` style for glyphs in
labels and `FontImageSource` for buttons.

## Responsive layout

| Width | Size class | Navigation | Content |
| --- | --- | --- | --- |
| < 640 | Compact | Phone: bottom tabs. Desktop: overlay sidebar | One column, options under the stage |
| 640–1099 | Medium | Overlay sidebar | Multi-column grids |
| ≥ 1100 | Expanded | Docked sidebar | Options beside the stage, up to four columns |

Phones always use tabs; tablets and desktops always use the sidebar. Window-level decisions use
`Window.SizeChanged`; components such as `AdaptiveGrid` and `ControlExample` react to their own width, so they
work in narrow windows and next to the sidebar.

## Components

| Component | Purpose |
| --- | --- |
| `ControlExample` | Stage, native badge, Default/Styled and LTR/RTL toggles, options, event output and code |
| `CodeViewer` | XAML and C# tabs, syntax highlighting, line numbers, copy |
| `NativeViewBadge` | Shows the platform type behind any view |
| `ItemCard` | Tappable tile for a gallery item, with a desktop context menu |
| `SectionHeader` | Section title with an optional action |
| `Chip` | Small metadata label |
| `AdaptiveGrid` | Equal-width columns that follow the available width and balance rows |
| `SwatchPicker` | Color choice for options panes |
| `DotGridView` | The stage's dotted canvas |

## Motion

Use motion sparingly: 150–250 ms with `Easing.CubicOut`, and only to explain a change of state.
