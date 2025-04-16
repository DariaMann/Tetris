#import <UIKit/UIKit.h>

extern "C" {
    bool _IsDarkTheme() {
        if (@available(iOS 12.0, *)) {
            UIUserInterfaceStyle style = UIScreen.mainScreen.traitCollection.userInterfaceStyle;
            return style == UIUserInterfaceStyleDark;
        }
        return false;
    }
}
