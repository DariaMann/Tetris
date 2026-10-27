public enum AnalyticType
{
    session_start = 0,
    
    game_start = 1,
    game_finish = 2,
    game_over = 3,
    game = 4,

    many_food_toggle = 5,
    move_through_walls_toggle = 6,
    acceleration_snake_toggle = 7,
    speed_snake_slider = 8,
    
    acceleration_tetris_toggle = 9,
    speed_tetris_slider = 10,
    
    button_no_ads_click = 11,
    no_ads_secret_code = 12,
    no_show_banner_secret_code = 13,
    
    button_change_figure_click = 14,
    count_change_blocks = 15,
    have_ads = 16,
    change = 17,
    
    app_paused = 18,
    app_resumed = 19,
    session_restart = 20,
    
    timestamp = 21,
    pause_duration_sec = 22,
    session_play_time = 23,
    play_time_sec = 24,
    gameplay_time_final = 25,
    record = 26,
    score = 27,
    is_win = 28,
    maximum = 29,
    
    interstitial_loaded = 30,
    interstitial_load_failed = 31,
    interstitial_show_failed = 32,
    interstitial_shown = 33,
    interstitial_closed = 34,
    interstitial_clicked = 35,
    interstitial_expired = 36,
    
    rewarded_loaded = 37,
    rewarded_load_failed = 38,
    rewarded_show_failed = 39,
    rewarded_shown = 40,
    rewarded_closed = 41,
    rewarded_finished = 42,
    rewarded_clicked = 43,
    rewarded_expired = 44,
    
    banner_loaded = 45,
    banner_load_failed = 46,
    banner_show_failed = 47,
    banner_shown = 48,
    banner_clicked = 49,
    banner_expired = 50,
    
    app_crash = 51,
    message = 52,
    stack_trace = 53,
    
    device_info = 54,
    platform = 55,
    device_model = 56,
    os_version = 57,
    language = 58,
    app_version = 59,
    build = 60,
    
    authenticate_services = 61,
    id_player = 62,
    name_player = 63,
    
    revive_tetris_click = 64,
    revive_snake_click = 65,
    revive_lines_click = 66,

    donation_panel_open = 67,
    donation_cloudtips_click = 68,
    donation_boosty_click = 69
}
