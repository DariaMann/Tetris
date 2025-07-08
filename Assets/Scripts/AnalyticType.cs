public enum AnalyticType
{
    session_start,
    
    game_start,
    game_finish,
    game_over,
    game,

    many_food_toggle,
    move_through_walls_toggle,
    acceleration_snake_toggle,
    speed_snake_slider,
    
    acceleration_tetris_toggle,
    speed_tetris_slider,
    
    button_no_ads_click,
    no_ads_secret_code,
    
    button_change_figure_click,
    count_change_blocks,
    have_ads,
    change,
    
    app_paused,
    app_resumed,
    session_restart,
    
    timestamp,
    pause_duration_sec,
    session_play_time,
    play_time_sec,
    gameplay_time_final,
    record,
    score,
    is_win,
    maximum,
    
    interstitial_loaded,
    interstitial_load_failed,
    interstitial_show_failed,
    interstitial_shown,
    interstitial_closed,
    interstitial_clicked,
    interstitial_expired,
    
    rewarded_loaded,
    rewarded_load_failed,
    rewarded_show_failed,
    rewarded_shown,
    rewarded_closed,
    rewarded_finished,
    rewarded_clicked,
    rewarded_expired,
    
    banner_loaded,
    banner_load_failed,
    banner_show_failed,
    banner_shown,
    banner_clicked,
    banner_expired,
    
    app_crash,
    message,
    stack_trace,
    
    device_info,
    platform,
    device_model,
    os_version,
    language,
    app_version,
    build,
    
    authenticate_services,
    id_player,
    name_player
}