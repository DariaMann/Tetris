#import <Foundation/Foundation.h>
#import <dispatch/dispatch.h>
#import <objc/message.h>

// YandexMobileAds 8.3.0 references the stored-property addressor from the
// AppMetricaLibraryAdapter Swift module. AppMetrica 6.5.1 builds without it.
// Keep the address stable and point it at AppMetrica's actual shared instance.
void *CombyAppMetricaSharedAddress(void)
    __asm__("_$s24AppMetricaLibraryAdapter09AnalyticscD0C6sharedACvau");

void *CombyAppMetricaSharedAddress(void)
{
    static id shared;
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        Class adapterClass = NSClassFromString(@"AMAAnalyticsLibraryAdapter");
        SEL selector = NSSelectorFromString(@"sharedInstance");
        id (*getter)(id, SEL) = (id (*)(id, SEL))objc_msgSend;
        shared = getter(adapterClass, selector);
    });
    return &shared;
}
