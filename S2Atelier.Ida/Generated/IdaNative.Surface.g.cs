#nullable enable

using S2Atelier.Ida;

namespace S2Atelier.Ida.Generated;

public static unsafe partial class IdaNative
{
    internal static delegate* unmanaged[Cdecl]<void*, void> _MD5Init;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, void> _MD5Update;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, long, byte> _add_auto_stkpnt;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte*, byte*, byte, int> _add_base_tils;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, void> _add_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _add_cref;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _add_dref;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _add_dword;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _add_encoding;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte*, byte, int, byte> _add_entry;
    internal static delegate* unmanaged[Cdecl]<void*, long, ushort, ulong, byte> _add_frame;
    internal static delegate* unmanaged[Cdecl]<ulong, long, ushort, ulong, byte> _add_frame_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, ulong, TypeInfo*, void*, uint, byte> _add_frame_member;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, ulong, TypeInfo*, void*, uint, byte> _add_frame_member_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, long, byte> _add_func_auto_stkpnt;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _add_func_ex;
    internal static delegate* unmanaged[Cdecl]<ulong, int, TypeInfo*, byte*, void> _add_func_regarg;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte*, byte*, byte*, int> _add_func_regvar;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _add_function_ex;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte*, byte*, byte*, uint, byte> _add_hidden_range;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, void*> _add_idc_class;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _add_idc_func;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _add_idc_gvar;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte> _add_mapping;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _add_qword;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void*, long, int, int, ulong> _add_refinfo_dref;
    internal static delegate* unmanaged[Cdecl]<void*, int, TypeInfo*, byte*, void> _add_regarg;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte*, byte*, byte*, int> _add_regvar;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte*, byte*, int, byte> _add_segm;
    internal static delegate* unmanaged[Cdecl]<void*, int, byte> _add_segment_ex;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _add_segment_translation;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte*, byte> _add_sourcefile;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _add_sourcefiles;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, nint, byte*> _add_spaces;
    internal static delegate* unmanaged[Cdecl]<void*, void*, long, int, byte> _add_stkvar;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int> _add_til;
    internal static delegate* unmanaged[Cdecl]<void*, int> _add_tryblk;
    internal static delegate* unmanaged[Cdecl]<ulong, long, byte> _add_user_stkpnt;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _add_word;
    internal static delegate* unmanaged[Cdecl]<ulong, long, long> _adjust_segment_diff;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _adjust_segment_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _align_down_to_stack;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _align_up_to_stack;
    internal static delegate* unmanaged[Cdecl]<void*, int, uint> _alloc_type_ordinals;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _allocate_selector;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte> _append_abi_opts;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _append_argloc;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte, byte> _append_cmt;
    internal static delegate* unmanaged[Cdecl]<QString*, long, byte, void> _append_disp;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _append_func_tail;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte> _append_func_tail_ea;
    internal static delegate* unmanaged[Cdecl]<QString*, long*, int, ulong*, int, ulong, long, byte, ulong> _append_struct_fields;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _append_tinfo_covered;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _append_to_flowchart;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _append_to_func_flow_chart;
    internal static delegate* unmanaged[Cdecl]<ulong, TypeInfo*, byte> _apply_callee_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte*, int, byte> _apply_cdecl;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte, byte> _apply_fixup;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong, byte, int> _apply_idasgn_to;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, uint, void> _apply_metadata;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte> _apply_named_type;
    internal static delegate* unmanaged[Cdecl]<ulong, TypeInfo*, byte*, byte> _apply_once_tinfo_and_name;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte> _apply_startup_sig;
    internal static delegate* unmanaged[Cdecl]<ulong, TypeInfo*, uint, byte> _apply_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, TypeInfo*, byte*, byte> _apply_tinfo_to_stkarg;
    internal static delegate* unmanaged[Cdecl]<byte**, void*, int> _asctoreal;
    internal static delegate* unmanaged[Cdecl]<uint*, byte*, byte> _atob32;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, byte> _atob64;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, byte> _atoea;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, int> _atos;
    internal static delegate* unmanaged[Cdecl]<int, int, byte> _attach_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _auto_apply_tail;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _auto_apply_type;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _auto_cancel;
    internal static delegate* unmanaged[Cdecl]<int*, ulong, ulong, ulong> _auto_get;
    internal static delegate* unmanaged[Cdecl]<byte> _auto_is_ok;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _auto_make_step;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, void> _auto_mark_range;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _auto_recreate_insn;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, void> _auto_unmark;
    internal static delegate* unmanaged[Cdecl]<byte> _auto_wait;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, nint> _auto_wait_range;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, uint, int, int, nuint> _b2a32;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, ulong, int, int, nuint> _b2a64;
    internal static delegate* unmanaged[Cdecl]<int, int, nuint> _b2a_width;
    internal static delegate* unmanaged[Cdecl]<byte**, byte> _back_char;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _backup_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, nuint, byte> _base64_decode;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, nuint, byte> _base64_encode;
    internal static delegate* unmanaged[Cdecl]<int, void> _begin_type_updating;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, int, nuint*, ulong> _bin_search;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _bitcount;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _bitcountr_zero;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, void*, nuint, byte, byte> _bitrange_t_extract_using_bitrange;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, void*, nuint, byte, byte> _bitrange_t_inject_using_bitrange;
    internal static delegate* unmanaged[Cdecl]<void*, uint, void*, byte> _bookmarks_t_erase;
    internal static delegate* unmanaged[Cdecl]<void*, void*, uint> _bookmarks_t_find_index;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, uint*, void*, byte> _bookmarks_t_get;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, ulong, void*, uint> _bookmarks_t_get_by_inode;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, uint, void*, byte> _bookmarks_t_get_desc;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _bookmarks_t_get_dirtree_id;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte*, byte*, void*, uint> _bookmarks_t_mark;
    internal static delegate* unmanaged[Cdecl]<void*, void*, uint> _bookmarks_t_size;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, uint, int, nuint> _btoa32;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, ulong, int, nuint> _btoa64;
    internal static delegate* unmanaged[Cdecl]<int, ulong, int, nuint> _btoa_width;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte*, void> _build_anon_type_name;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*> _build_loaders_list;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte*, void> _build_plugin_options;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _build_snapshot_tree;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, long, nint> _build_stkvar_name;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, long, nint> _build_stkvar_name_ea;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, ulong, void> _build_stkvar_xrefs;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, ulong, void> _build_stkvar_xrefs_ea;
    internal static delegate* unmanaged[Cdecl]<void> _build_strlist;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _calc_arglocs;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _calc_basevalue;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _calc_bg_color;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, TypeInfo*, int, nint> _calc_c_cpp_name;
    internal static delegate* unmanaged[Cdecl]<uint, void*, nuint, uint> _calc_crc32;
    internal static delegate* unmanaged[Cdecl]<void*, int, int, ulong> _calc_dataseg;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int, int> _calc_def_align;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _calc_file_crc32;
    internal static delegate* unmanaged[Cdecl]<ushort, int> _calc_fixup_size;
    internal static delegate* unmanaged[Cdecl]<void*, long, void*, void*, long> _calc_frame_offset;
    internal static delegate* unmanaged[Cdecl]<ulong, long, void*, void*, long> _calc_frame_offset_ea;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void*, ulong> _calc_func_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _calc_func_size;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _calc_func_size_ea;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, void*, ulong> _calc_function_metadata;
    internal static delegate* unmanaged[Cdecl]<int, int> _calc_idasgn_state;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _calc_max_align;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _calc_max_item_end;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _calc_min_align;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, byte, int> _calc_number_of_children;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _calc_offset_base;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _calc_prefix_color;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _calc_probable_base_by_value;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong*, ulong, void*, long, byte> _calc_reference_data;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _calc_retloc;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, ulong> _calc_stkvar_struc_offset;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, int, ulong> _calc_stkvar_struc_offset_ea;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, void*, byte> _calc_switch_cases;
    internal static delegate* unmanaged[Cdecl]<void*, ulong*, ulong> _calc_thunk_func_target;
    internal static delegate* unmanaged[Cdecl]<void*, ulong*, ulong> _calc_thunk_function_target;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _calc_tinfo_gaps;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int, byte> _calc_varglocs;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _call_system;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _can_be_off32;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte> _can_define_item;
    internal static delegate* unmanaged[Cdecl]<byte, byte*, byte*> _cfg_get_cc_parm;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, byte*> _cfgopt_t__apply;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, void*, byte*> _cfgopt_t__apply2;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, void*, void*, byte*> _cfgopt_t__apply3;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, int, byte> _change_codepage;
    internal static delegate* unmanaged[Cdecl]<void*, byte, int> _change_segment_status;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int> _change_segment_status_by_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, int> _change_storage_type;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int> _check_flat_jump_table;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _check_spoiled_jpt;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, void*, byte> _choose_ioport_device2;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*, uint, void*, uint> _choose_local_tinfo;
    internal static delegate* unmanaged[Cdecl]<int*, void*, byte*, void*, uint, void*, uint> _choose_local_tinfo_and_delta;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, int, void*, byte> _choose_named_type;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _chunk_size;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _chunk_start;
    internal static delegate* unmanaged[Cdecl]<int, int> _cleanup_appcall;
    internal static delegate* unmanaged[Cdecl]<void*, void> _cleanup_argloc;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte*, uint, byte> _cleanup_name;
    internal static delegate* unmanaged[Cdecl]<void> _clear_strlist;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void> _clear_tinfo_t;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, void> _cliopts_t_add;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*> _cliopts_t_find_long;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void*> _cliopts_t_find_short;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void> _cliopts_t_usage;
    internal static delegate* unmanaged[Cdecl]<byte, void> _close_database;
    internal static delegate* unmanaged[Cdecl]<void*, void> _close_linput;
    internal static delegate* unmanaged[Cdecl]<byte*> _closing_comment;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, void> _clr_abits;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _clr_lzero;
    internal static delegate* unmanaged[Cdecl]<int, void*> _clr_module_data;
    internal static delegate* unmanaged[Cdecl]<ulong, int, uint, void> _clr_node_info;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _clr_op_type;
    internal static delegate* unmanaged[Cdecl]<void*, void*, QString*, void> _code_highlight_block;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, ulong, void> _combine_regs_jpt;
    internal static delegate* unmanaged[Cdecl]<void*, uint, void*, int, int> _compact_numbered_types;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _compact_til;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _compare_arglocs;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _compare_bpt_locs;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _compare_tinfo;
    internal static delegate* unmanaged[Cdecl]<byte*, QString*, int, byte> _compile_idc_file;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, QString*, void*, byte, byte> _compile_idc_snippet;
    internal static delegate* unmanaged[Cdecl]<byte*, QString*, void*, byte, byte> _compile_idc_text;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte, byte> _construct_macro;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _copy_argloc;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _copy_debug_event;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _copy_idcv;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, uint> _copy_named_type;
    internal static delegate* unmanaged[Cdecl]<int, int, byte, void> _copy_sreg_ranges;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, TypeInfo*, void> _copy_tinfo_t;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, byte, ulong> _correct_address;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int> _cpu2ieee;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _create_16bit_data;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _create_32bit_data;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _create_align;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*> _create_bytearray_linput;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, byte> _create_data;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*> _create_dirtree;
    internal static delegate* unmanaged[Cdecl]<void> _create_filename_cmt;
    internal static delegate* unmanaged[Cdecl]<void*, void> _create_func_flow_chart;
    internal static delegate* unmanaged[Cdecl]<void*, void*> _create_generic_linput;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _create_idcv_ref;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, int> _create_insn;
    internal static delegate* unmanaged[Cdecl]<byte**, nuint, void*, uint, void*> _create_lexer;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*> _create_memory_linput;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _create_multirange_func_flow_chart;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _create_multirange_qflow_chart;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, byte*, byte, uint, void*, byte, void*> _create_nodeval_merge_handler2;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, byte*, void*, nuint, byte, void> _create_nodeval_merge_handlers;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, byte*, void*, nuint, byte, void> _create_nodeval_merge_handlers2;
    internal static delegate* unmanaged[Cdecl]<QString*, int, nint> _create_numbered_type_name;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, void*> _create_outctx;
    internal static delegate* unmanaged[Cdecl]<void*, void> _create_qflow_chart;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, void*, nuint, void> _create_std_modmerge_handlers;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, void*, nuint, void> _create_std_modmerge_handlers2;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint, int, byte> _create_strlit;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte> _create_switch_table;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _create_switch_xrefs;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, byte, byte, void*, byte> _create_tinfo;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte> _create_undo_point;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _create_xrefs_from;
    internal static delegate* unmanaged[Cdecl]<void*, nint, int, void*> _create_zip_linput;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, TypeInfo*, void*, nuint, int> _dbg_appcall;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, nint> _dbg_get_input_path;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int> _decode_insn;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte*, ulong> _decode_preceding_insn;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _decode_prev_insn;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte, uint, TypeInfo*, byte> _decorate_name;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _deep_copy_idcv;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, long, TypeInfo*, void*, byte> _define_stkvar;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, long, TypeInfo*, void*, byte> _define_stkvar_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_aflags;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte, byte> _del_cref;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _del_debug_names;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _del_dref;
    internal static delegate* unmanaged[Cdecl]<int, byte> _del_encoding;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _del_extra_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_fixup;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _del_frame;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _del_frame_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _del_func;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte*, int> _del_func_regvar;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _del_func_stkpnt;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _del_hidden_range;
    internal static delegate* unmanaged[Cdecl]<int, int> _del_idasgn;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _del_idc_func;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _del_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _del_item_color;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, void*, byte> _del_items;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_mapping;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, byte> _del_named_type;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void> _del_node_info;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte> _del_numbered_type;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _del_refinfo;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte*, int> _del_regvar;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _del_segm;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_segment_translations;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_selector;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_source_linnum;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _del_sourcefile;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _del_sreg_range;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _del_stkpnt;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_str_type;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_switch_info;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _del_til;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, QString*, byte, byte> _del_tinfo_attr;
    internal static delegate* unmanaged[Cdecl]<void*, void> _del_tryblks;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _del_value;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _delete_all_xrefs_from;
    internal static delegate* unmanaged[Cdecl]<void*, void> _delete_dirtree;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void> _delete_extra_cmts;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _delete_frame_members;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte> _delete_frame_members_ea;
    internal static delegate* unmanaged[Cdecl]<void> _delete_imports;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _delete_switch_table;
    internal static delegate* unmanaged[Cdecl]<int, byte> _delinf;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, uint, int, int> _demangle_name;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*> _deref_idcv;
    internal static delegate* unmanaged[Cdecl]<ulong*, TypeInfo*, ulong*, byte> _deref_ptr;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _deserialize_dynamic_register_set;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _deserialize_insn;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, byte**, byte**, byte**, byte*, byte> _deserialize_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, void> _destroy_lexer;
    internal static delegate* unmanaged[Cdecl]<int, void> _destroy_moddata_merge_handlers;
    internal static delegate* unmanaged[Cdecl]<int, int, byte> _detach_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, byte> _detach_tinfo_t;
    internal static delegate* unmanaged[Cdecl]<void> _determine_rtl;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, uint, byte> _diff_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void> _diff_source_merge_region;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _dirtree_add_event_handler;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, int, void*, void*, int> _dirtree_bulk_move;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int> _dirtree_bulk_remove;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, nint, int> _dirtree_change_rank;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _dirtree_chdir;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _dirtree_errstr;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int> _dirtree_find_entry;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, byte> _dirtree_findfirst;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _dirtree_findnext;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte, int> _dirtree_fold_common_prefix;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, void*, uint, byte> _dirtree_get_abspath_by_cursor;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte*, byte> _dirtree_get_abspath_by_relpath;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint> _dirtree_get_dir_size;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, void*, void> _dirtree_get_entry_attrs;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, void*, uint, byte> _dirtree_get_entry_name;
    internal static delegate* unmanaged[Cdecl]<void*, byte*> _dirtree_get_id;
    internal static delegate* unmanaged[Cdecl]<void*, byte*> _dirtree_get_nodename;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void> _dirtree_get_parent_cursor;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void*, nint> _dirtree_get_rank;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, void> _dirtree_getcwd;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _dirtree_is_dir_ordered;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _dirtree_is_orderable;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte, int> _dirtree_link;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte, int> _dirtree_link_inode;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, void> _dirtree_make_cursor;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _dirtree_mkdir;
    internal static delegate* unmanaged[Cdecl]<void*, void*> _dirtree_new_shadow_dirtree;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _dirtree_remove_event_handler;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, int> _dirtree_rename;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void> _dirtree_resolve_cursor;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, void> _dirtree_resolve_path;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _dirtree_rmdir;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _dirtree_set_id;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _dirtree_set_nodename;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nint> _dirtree_traverse;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int> _disable_flags;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _display_gdl;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, byte*> _dstr_tinfo;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _dummy_name_ea;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, int, byte> _dump_func_type_data;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _ea2node;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, ulong, nuint> _ea2str;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte, int> _eadd;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void> _echsize;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _ecmp;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int> _ediv;
    internal static delegate* unmanaged[Cdecl]<long*, void*, byte, int> _eetol;
    internal static delegate* unmanaged[Cdecl]<long*, void*, byte, int> _eetol64;
    internal static delegate* unmanaged[Cdecl]<ulong*, void*, byte, int> _eetol64u;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, int> _eldexp;
    internal static delegate* unmanaged[Cdecl]<long, void*, void> _eltoe;
    internal static delegate* unmanaged[Cdecl]<long, void*, void> _eltoe64;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _eltoe64u;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int> _emul;
    internal static delegate* unmanaged[Cdecl]<byte, byte> _enable_auto;
    internal static delegate* unmanaged[Cdecl]<byte, void> _enable_console_messages;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, int> _enable_flags;
    internal static delegate* unmanaged[Cdecl]<void*, byte, byte> _enable_numbered_types;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _end_ea2node;
    internal static delegate* unmanaged[Cdecl]<int, void> _end_type_updating;
    internal static delegate* unmanaged[Cdecl]<int, void*, void*, int> _enum_import_names;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _enum_type_data_t__get_max_serial;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _enum_type_data_t__get_value_repr;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _enum_type_data_t__set_value_repr;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte*, void*, int> _enumerate_files;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, ulong> _enumerate_segments_with_selector_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte*, nuint, int, byte> _equal_bytes;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte*, QString*, byte> _eval_expr;
    internal static delegate* unmanaged[Cdecl]<long*, ulong, byte*, QString*, byte> _eval_expr_long;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte*, QString*, byte> _eval_idc_expr;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, QString*, void*, byte> _eval_idc_snippet;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte> _exec_system_script;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte, ulong> _extend_sign;
    internal static delegate* unmanaged[Cdecl]<void*, byte**, byte, byte> _extract_argloc;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_extra_cmts_from_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_frame_desc_from_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_insn_cmts_from_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_insn_opreprs_from_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_insn_opreprs_from_metadata_ex;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte**, byte, byte> _extract_module_from_archive;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, nint> _extract_name;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_type_from_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, void> _extract_user_stkpnts_from_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, int> _fc_calc_block_type;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, int> _fc_calc_func_block_type;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte*, int, int, int, ulong> _find_binary;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte, int, ulong> _find_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte, int, ulong> _find_byter;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _find_code;
    internal static delegate* unmanaged[Cdecl]<byte*, uint> _find_custom_callcnv;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _find_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _find_custom_data_type;
    internal static delegate* unmanaged[Cdecl]<byte*, ushort> _find_custom_fixup;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _find_custom_refinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _find_data;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _find_defined;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, ulong> _find_defjump_from_table;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int*, ulong> _find_error;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*> _find_extlang;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong> _find_free_chunk;
    internal static delegate* unmanaged[Cdecl]<ulong> _find_free_selector;
    internal static delegate* unmanaged[Cdecl]<void*, int, int> _find_func_bounds;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, ulong, byte*, byte*, nint> _find_func_regvar;
    internal static delegate* unmanaged[Cdecl]<void*, int, int> _find_function_bounds;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _find_idc_class;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, byte> _find_idc_func;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _find_idc_gvar;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, int*, ulong> _find_imm;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void*> _find_ioport;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nuint, void*> _find_ioport_bit;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _find_jtable_size;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _find_not_func;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int*, ulong> _find_notype;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, void*> _find_plugin;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte*, int, ulong> _find_reg_access;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong, int, int> _find_reg_value;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int, byte> _find_reg_value_info;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte*, int, byte> _find_regname_value_info;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte*, byte*, void*> _find_regvar;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _find_selector;
    internal static delegate* unmanaged[Cdecl]<long*, ulong, int, int> _find_sp_value;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int*, ulong> _find_suspop;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _find_syseh;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int, byte*, int, ulong> _find_text;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int> _find_tinfo_udt_member;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _find_unknown;
    internal static delegate* unmanaged[Cdecl]<void*, byte*> _first_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<void*, int, byte*> _first_named_type;
    internal static delegate* unmanaged[Cdecl]<int> _flush_buffers;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _fopenA;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _fopenM;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _fopenRB;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _fopenRT;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _fopenWB;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _fopenWT;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int, int> _for_all_arglocs;
    internal static delegate* unmanaged[Cdecl]<void*, byte, nint> _for_all_extlangs;
    internal static delegate* unmanaged[Cdecl]<byte, ulong, byte> _forget_problem;
    internal static delegate* unmanaged[Cdecl]<void*, void*, TypeInfo*, void*, void*, byte> _format_cdata;
    internal static delegate* unmanaged[Cdecl]<QString*, byte**, nuint, uint, int, byte> _format_charlit;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, ulong, uint, byte> _format_timestamp;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int, int> _freadbytes;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_debug_event;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_dll;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_idcv;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_loaders_list;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_regarg;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_regvar;
    internal static delegate* unmanaged[Cdecl]<void*, void> _free_til;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _func_does_return;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte> _func_has_stkframe_hole;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, void*, byte> _func_item_iterator_decode_preceding_insn;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _func_item_iterator_decode_prev_insn;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _func_item_iterator_next;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _func_item_iterator_prev;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _func_item_iterator_succ;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _func_parent_iterator_set;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, byte> _func_tail_iterator_set;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _func_tail_iterator_set_ea;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, void*, byte> _function_item_iterator_decode_preceding_insn;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _function_item_iterator_decode_prev_insn;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _function_item_iterator_next;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _function_item_iterator_prev;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _function_item_iterator_succ;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_parent_iterator_first;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_parent_iterator_last;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_parent_iterator_next;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _function_parent_iterator_parent;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_parent_iterator_prev;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _function_parent_iterator_set;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _function_tail_iterator_chunk;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_tail_iterator_first;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_tail_iterator_last;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_tail_iterator_main;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_tail_iterator_next;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _function_tail_iterator_prev;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _function_tail_iterator_set;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _function_tail_iterator_set_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _function_tail_iterator_set_range;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int, int> _fwritebytes;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, ulong, ulong, int, int, byte> _gen_complex_call_chart;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte, uint, TypeInfo*, byte> _gen_decorate_name;
    internal static delegate* unmanaged[Cdecl]<void*, int> _gen_exe_file;
    internal static delegate* unmanaged[Cdecl]<int, void*, ulong, ulong, int, int> _gen_file;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, void> _gen_fix_fixups;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, void*, ulong, ulong, int, byte> _gen_flow_graph;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, ulong, ulong, ulong, int, byte> _gen_flow_graph_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _gen_gdl;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, byte> _gen_rand_buf;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, int, byte> _gen_simple_call_chart;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void*, void*, void> _gen_use_arg_tinfos;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, byte> _generate_disasm_line;
    internal static delegate* unmanaged[Cdecl]<void*, int*, ulong, int, int, int> _generate_disassembly;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_16bit;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_32bit;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_64bit;
    internal static delegate* unmanaged[Cdecl]<QString*, nint> _get_abi_name;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_aflags;
    internal static delegate* unmanaged[Cdecl]<void*, uint, uint> _get_alias_target;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_arg_addrs;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint> _get_array_parameters;
    internal static delegate* unmanaged[Cdecl]<void*> _get_ash;
    internal static delegate* unmanaged[Cdecl]<AutoDisplay*, byte> _get_auto_display;
    internal static delegate* unmanaged[Cdecl]<int> _get_auto_state;
    internal static delegate* unmanaged[Cdecl]<int> _get_available_core_count;
    internal static delegate* unmanaged[Cdecl]<void*, int> _get_basic_file_type;
    internal static delegate* unmanaged[Cdecl]<void*, void> _get_builtin_widgets_state;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _get_byte;
    internal static delegate* unmanaged[Cdecl]<void*, nint, ulong, int, void*, nint> _get_bytes;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte, nint> _get_cmt;
    internal static delegate* unmanaged[Cdecl]<byte, byte*> _get_compiler_abbr;
    internal static delegate* unmanaged[Cdecl]<byte, byte*> _get_compiler_name;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void> _get_compilers;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _get_config_value;
    internal static delegate* unmanaged[Cdecl]<int, uint, uint, byte> _get_cp_validity;
    internal static delegate* unmanaged[Cdecl]<void*> _get_current_extlang;
    internal static delegate* unmanaged[Cdecl]<int> _get_current_idasgn;
    internal static delegate* unmanaged[Cdecl]<uint, void*> _get_custom_callcnv;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint> _get_custom_callcnvs;
    internal static delegate* unmanaged[Cdecl]<int, void*> _get_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<void*, int, int> _get_custom_data_formats;
    internal static delegate* unmanaged[Cdecl]<int, void*> _get_custom_data_type;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int> _get_custom_data_type_ids;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, int> _get_custom_data_types;
    internal static delegate* unmanaged[Cdecl]<int, void*> _get_custom_refinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, ulong> _get_data_elsize;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong, ulong, byte> _get_data_value;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _get_db_byte;
    internal static delegate* unmanaged[Cdecl]<nint> _get_dbctx_id;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_dbctx_qty;
    internal static delegate* unmanaged[Cdecl]<uint*, ulong, byte> _get_dbg_byte;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong*, int, nint> _get_debug_name;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _get_debug_name_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, void> _get_debug_names;
    internal static delegate* unmanaged[Cdecl]<void*, nuint> _get_debugger_plugins;
    internal static delegate* unmanaged[Cdecl]<int, int> _get_default_encoding_idx;
    internal static delegate* unmanaged[Cdecl]<int> _get_default_radix;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _get_default_reftype;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_dirty_infos;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _get_dtype_by_size;
    internal static delegate* unmanaged[Cdecl]<byte, ulong> _get_dtype_flag;
    internal static delegate* unmanaged[Cdecl]<byte, nuint> _get_dtype_size;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_dword;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nint> _get_ea_diffpos_name;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, void*, nint> _get_ea_name;
    internal static delegate* unmanaged[Cdecl]<void*> _get_eah;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, ulong, nint> _get_edm_by_tid;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, long> _get_effective_spd;
    internal static delegate* unmanaged[Cdecl]<byte*> _get_elf_debug_file_directory;
    internal static delegate* unmanaged[Cdecl]<int, int> _get_encoding_bpu;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _get_encoding_bpu_by_name;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _get_encoding_name;
    internal static delegate* unmanaged[Cdecl]<int> _get_encoding_qty;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_entry;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nint> _get_entry_forwarder;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nint> _get_entry_name;
    internal static delegate* unmanaged[Cdecl]<nuint, ulong> _get_entry_ordinal;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_entry_qty;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong, int, ulong> _get_enum_id;
    internal static delegate* unmanaged[Cdecl]<QString*, TypeInfo*, int, ulong, byte> _get_enum_member_expr;
    internal static delegate* unmanaged[Cdecl]<byte*, int, byte*> _get_errdesc;
    internal static delegate* unmanaged[Cdecl]<int, nuint> _get_error_data;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _get_error_string;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, nint> _get_extra_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_fchunk;
    internal static delegate* unmanaged[Cdecl]<int, ulong> _get_fchunk_ea_by_num;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_fchunk_info;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _get_fchunk_num;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_fchunk_qty;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_fchunk_start;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _get_file_ext;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, nuint> _get_file_type_name;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_first_cref_from;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_first_cref_to;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_first_dref_from;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_first_dref_to;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_first_fcref_from;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_first_fcref_to;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_first_fixup_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int> _get_first_free_extra_cmtidx;
    internal static delegate* unmanaged[Cdecl]<void*> _get_first_hidden_range;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_first_hidden_range_ea;
    internal static delegate* unmanaged[Cdecl]<void*> _get_first_seg;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_first_segment_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_fixup;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, void*, byte*> _get_fixup_desc;
    internal static delegate* unmanaged[Cdecl]<ushort, void*> _get_fixup_handler;
    internal static delegate* unmanaged[Cdecl]<ulong, ushort, ulong> _get_fixup_value;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte> _get_fixups;
    internal static delegate* unmanaged[Cdecl]<nuint, ulong> _get_flags_by_size;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _get_flags_ex;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, nint> _get_forced_operand;
    internal static delegate* unmanaged[Cdecl]<void*, ushort, int> _get_fpvalue_kind;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, void> _get_frame_part;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_frame_part_ea;
    internal static delegate* unmanaged[Cdecl]<void*, int> _get_frame_retsize;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _get_frame_retsize_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _get_frame_size;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_frame_size_ea;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, long*, void*, void*, long, nint> _get_frame_var;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _get_free_disk_space;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_func;
    internal static delegate* unmanaged[Cdecl]<void*, int> _get_func_bitness;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _get_func_bitness_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int> _get_func_chunknum;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int> _get_func_chunknum_ea;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte, nint> _get_func_cmt;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte, nint> _get_func_cmt_ea;
    internal static delegate* unmanaged[Cdecl]<nuint, ulong> _get_func_ea_by_num;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, long> _get_func_effective_spd;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_func_entry_info;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, int, byte> _get_func_entry_info_by_num;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_func_flags;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, byte> _get_func_frame;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, byte> _get_func_frame_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_func_llabel_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_func_llabels;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nint> _get_func_name;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _get_func_num;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_func_qty;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong> _get_func_ranges;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _get_func_ranges_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nuint, byte> _get_func_regarg;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_func_regarg_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_func_regargs;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint, byte> _get_func_regvar;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_func_regvar_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_func_regvars;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, long> _get_func_sp_delta;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, long> _get_func_spd;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_func_start;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_func_stkpnt_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_func_stkpnts;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_func_tail_info;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_func_tail_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_func_tails;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_group_selector;
    internal static delegate* unmanaged[Cdecl]<void*> _get_hexdsp;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_hidden_range;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_hidden_range_info;
    internal static delegate* unmanaged[Cdecl]<void*, int, byte> _get_hidden_range_info_by_num;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _get_hidden_range_num;
    internal static delegate* unmanaged[Cdecl]<int> _get_hidden_range_qty;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, int> _get_ida_subdirs;
    internal static delegate* unmanaged[Cdecl]<nuint*, ulong*, void*, TypeInfo*, nuint*, byte> _get_idainfo_by_type;
    internal static delegate* unmanaged[Cdecl]<ulong*, void*, void*, ulong, byte> _get_idainfo_by_udm;
    internal static delegate* unmanaged[Cdecl]<QString*, QString*, int, int> _get_idasgn_desc;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, byte*, byte> _get_idasgn_header_by_short_name;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte> _get_idasgn_path_by_short_name;
    internal static delegate* unmanaged[Cdecl]<int> _get_idasgn_qty;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, nint> _get_idasgn_title;
    internal static delegate* unmanaged[Cdecl]<void*> _get_idati;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte*> _get_idc_filename;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, byte, int> _get_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, int> _get_idcv_class_name;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, ulong, int, int> _get_idcv_slice;
    internal static delegate* unmanaged[Cdecl]<void*> _get_idp_descs;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*> _get_idp_name;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong, int, ulong, void*, nuint> _get_immvals;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_import_entry;
    internal static delegate* unmanaged[Cdecl]<QString*, int, byte> _get_import_module_name;
    internal static delegate* unmanaged[Cdecl]<uint> _get_import_module_qty;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_ind_purged;
    internal static delegate* unmanaged[Cdecl]<QString*, void> _get_install_root;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_item_color;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_item_end;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, byte, ulong> _get_item_flag;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_item_refinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, int, ulong> _get_jtable_target;
    internal static delegate* unmanaged[Cdecl]<void*> _get_last_hidden_range;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_last_hidden_range_ea;
    internal static delegate* unmanaged[Cdecl]<int> _get_last_pfxlen;
    internal static delegate* unmanaged[Cdecl]<void*> _get_last_seg;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_last_segment_ea;
    internal static delegate* unmanaged[Cdecl]<int*, int*, int*, byte> _get_library_version;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, nint> _get_loader_name;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _get_loader_name_from_dll;
    internal static delegate* unmanaged[Cdecl]<int> _get_logical_core_count;
    internal static delegate* unmanaged[Cdecl]<QString*, byte> _get_login_name;
    internal static delegate* unmanaged[Cdecl]<int> _get_lookback;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _get_mangled_name_type;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nint> _get_manual_insn;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong*, ulong*, nuint, byte> _get_mapping;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_mappings_qty;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int, nuint> _get_max_strlit_length;
    internal static delegate* unmanaged[Cdecl]<int, void*> _get_module_data;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_name_base_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _get_name_color;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, ulong> _get_name_ea;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, ulong, ulong, int, nint> _get_name_expr;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong, byte*, int> _get_name_value;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, byte**, byte**, byte**, byte**, int*, uint*, int> _get_named_type;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _get_named_type_tid;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_cref_from;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_cref_to;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_dref_from;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_dref_to;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_next_fchunk;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_next_fchunk_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_next_fchunk_info;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_fcref_from;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_fcref_to;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_next_fixup_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_next_func;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _get_next_func_addr;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_next_func_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_next_function_addr;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_next_hidden_range;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_next_hidden_range_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_next_seg;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_next_segment_ea;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, nint> _get_nice_colored_name;
    internal static delegate* unmanaged[Cdecl]<nuint, ulong> _get_nlist_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_nlist_idx;
    internal static delegate* unmanaged[Cdecl]<nuint, byte*> _get_nlist_name;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_nlist_size;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_node_info;
    internal static delegate* unmanaged[Cdecl]<ulong> _get_nsec_stamp;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte**, byte**, byte**, byte**, int*, byte> _get_numbered_type;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte*> _get_numbered_type_name;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, byte> _get_octet;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, void*, ulong, long, int, int> _get_offset_expr;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, ulong, long, int, int> _get_offset_expression;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, int, byte> _get_op_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, ulong, void*> _get_opinfo;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _get_ordinal_limit;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_original_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_original_dword;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_original_qword;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_original_word;
    internal static delegate* unmanaged[Cdecl]<int> _get_outfile_encoding_idx;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte*, byte> _get_parser_option;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _get_path;
    internal static delegate* unmanaged[Cdecl]<void*> _get_ph;
    internal static delegate* unmanaged[Cdecl]<int> _get_physical_core_count;
    internal static delegate* unmanaged[Cdecl]<int*, int*, int, void*> _get_place_class;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _get_place_class_id;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _get_plugin_options;
    internal static delegate* unmanaged[Cdecl]<void*> _get_plugins;
    internal static delegate* unmanaged[Cdecl]<ulong, TypeInfo*, ulong> _get_possible_item_varsize;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, nint> _get_predef_insn_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_prev_fchunk;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_prev_fchunk_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_prev_fchunk_info;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_prev_fixup_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_prev_func;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _get_prev_func_addr;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_prev_func_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _get_prev_function_addr;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_prev_hidden_range;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_prev_hidden_range_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_prev_seg;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_prev_segment_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_prev_sreg_range;
    internal static delegate* unmanaged[Cdecl]<byte, ulong, ulong> _get_problem;
    internal static delegate* unmanaged[Cdecl]<QString*, byte, ulong, nint> _get_problem_desc;
    internal static delegate* unmanaged[Cdecl]<byte, byte, byte*> _get_problem_name;
    internal static delegate* unmanaged[Cdecl]<int> _get_qerrno;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_qword;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int> _get_radix;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _get_realtype;
    internal static delegate* unmanaged[Cdecl]<QString*, byte> _get_redo_action_label;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_refinfo;
    internal static delegate* unmanaged[Cdecl]<void*, void> _get_refinfo_descs;
    internal static delegate* unmanaged[Cdecl]<nuint, byte> _get_reftype_by_size;
    internal static delegate* unmanaged[Cdecl]<QString*, int, nuint, int, nint> _get_reg_name;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, nint> _get_root_filename;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _get_segm_base;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _get_segm_by_name;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _get_segm_by_sel;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, nint> _get_segm_class;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, int, nint> _get_segm_name;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _get_segm_num;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _get_segm_para;
    internal static delegate* unmanaged[Cdecl]<int> _get_segm_qty;
    internal static delegate* unmanaged[Cdecl]<byte, byte*> _get_segment_alignment;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_segment_base;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nint> _get_segment_class;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte, nint> _get_segment_cmt;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte, nint> _get_segment_cmt_by_ea;
    internal static delegate* unmanaged[Cdecl]<byte, byte*> _get_segment_combination;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_segment_ea;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _get_segment_ea_by_name;
    internal static delegate* unmanaged[Cdecl]<int, ulong> _get_segment_ea_by_num;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_segment_ea_by_sel;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_segment_info;
    internal static delegate* unmanaged[Cdecl]<void*, int, int, byte> _get_segment_info_by_num;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, nint> _get_segment_name;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_segment_para;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint> _get_segment_translations;
    internal static delegate* unmanaged[Cdecl]<QString*, byte> _get_selected_parser_name;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_selector_qty;
    internal static delegate* unmanaged[Cdecl]<void*> _get_server_connection;
    internal static delegate* unmanaged[Cdecl]<int, void*> _get_server_connection2;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_source_linnum;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte*> _get_sourcefile;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, void*, byte> _get_sourcefile_by_ea;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_sourcefiles_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, long> _get_sp_delta;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, long> _get_spd;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, int, byte> _get_special_folder;
    internal static delegate* unmanaged[Cdecl]<void*, uint*, nuint, int> _get_spoiled_reg;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _get_sreg;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _get_sreg_range;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int> _get_sreg_range_num;
    internal static delegate* unmanaged[Cdecl]<int, nuint> _get_sreg_ranges_qty;
    internal static delegate* unmanaged[Cdecl]<int, void*> _get_std_dirtree;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte> _get_stkarg_area_info;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, int, byte> _get_stock_tinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_str_type;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_strid;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, byte> _get_strlist_item;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, byte> _get_strlist_item_ex;
    internal static delegate* unmanaged[Cdecl]<void*> _get_strlist_options;
    internal static delegate* unmanaged[Cdecl]<nuint> _get_strlist_qty;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, nuint, int, nuint*, int, nint> _get_strlit_contents;
    internal static delegate* unmanaged[Cdecl]<ulong*, long*, ulong, int, int> _get_stroff_path;
    internal static delegate* unmanaged[Cdecl]<long*, long*, ulong*, ulong, int, int> _get_struct_operand;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint> _get_switch_info;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_tail_owner;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint, ulong> _get_tail_referer;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint> _get_tail_referer_qty;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _get_tail_referers;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte> _get_tid_name;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_tid_ordinal;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, byte> _get_tinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, void*, byte, byte> _get_tinfo_attr;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte, byte> _get_tinfo_attrs;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, byte*, nint> _get_tinfo_by_edm_name;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, byte> _get_tinfo_by_flags;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void*, byte> _get_tinfo_details;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, nuint> _get_tinfo_pdata;
    internal static delegate* unmanaged[Cdecl]<ulong, int, nuint> _get_tinfo_property;
    internal static delegate* unmanaged[Cdecl]<ulong, int, nuint, nuint, nuint, nuint, nuint> _get_tinfo_property4;
    internal static delegate* unmanaged[Cdecl]<uint*, ulong, int, nuint> _get_tinfo_size;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, byte, ulong> _get_tinfo_tid;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint> _get_tryblks;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, byte> _get_type_by_tid;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _get_type_ordinal;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, nint> _get_udm_by_fullname;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, ulong, nint> _get_udm_by_tid;
    internal static delegate* unmanaged[Cdecl]<QString*, byte> _get_undo_action_label;
    internal static delegate* unmanaged[Cdecl]<byte*> _get_user_idadir;
    internal static delegate* unmanaged[Cdecl]<byte**, uint> _get_utf8_char;
    internal static delegate* unmanaged[Cdecl]<uint, ulong> _get_vftable_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, uint> _get_vftable_ordinal;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_wide_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_wide_dword;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _get_wide_word;
    internal static delegate* unmanaged[Cdecl]<ulong, ushort> _get_word;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint> _get_xrefpos;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _get_zero_ranges;
    internal static delegate* unmanaged[Cdecl]<int, nuint> _getinf;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, nint> _getinf_buf;
    internal static delegate* unmanaged[Cdecl]<int, uint, byte> _getinf_flag;
    internal static delegate* unmanaged[Cdecl]<QString*, int, nint> _getinf_str;
    internal static delegate* unmanaged[Cdecl]<int, void*> _getn_fchunk;
    internal static delegate* unmanaged[Cdecl]<nuint, void*> _getn_func;
    internal static delegate* unmanaged[Cdecl]<int, void*> _getn_hidden_range;
    internal static delegate* unmanaged[Cdecl]<ulong*, ulong*, int, byte> _getn_selector;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, byte> _getn_sourcefile;
    internal static delegate* unmanaged[Cdecl]<void*, int, int, byte> _getn_sreg_range;
    internal static delegate* unmanaged[Cdecl]<int, void*> _getnseg;
    internal static delegate* unmanaged[Cdecl]<ulong, void*> _getseg;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte*, byte*> _getsysfile;
    internal static delegate* unmanaged[Cdecl]<void*, int, int, uint> _guess_func_cc;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, int> _guess_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, int, void*, void*, void*, void*, int, int> _h2ti;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ushort, uint, byte> _handle_fixups_in_macro;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _has_backup_metadata;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _has_external_refs;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _has_external_refs_ea;
    internal static delegate* unmanaged[Cdecl]<ushort, uint, byte> _has_insn_feature;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _has_jump_or_flow_xref;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*, ulong, nuint> _hexplace_t__ea2str;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int, byte*, byte, void> _hexplace_t__out_one_item;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _hide_name;
    internal static delegate* unmanaged[Cdecl]<int, void*, void*, int, byte> _hook_event_listener;
    internal static delegate* unmanaged[Cdecl]<int, void*, void*, byte> _hook_to_notification_point;
    internal static delegate* unmanaged[Cdecl]<byte*, int, void> _ida_checkmem;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _idadir;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, int, byte> _idb_utf8;
    internal static delegate* unmanaged[Cdecl]<void*, int> _idcv_float;
    internal static delegate* unmanaged[Cdecl]<void*, int> _idcv_int64;
    internal static delegate* unmanaged[Cdecl]<void*, int> _idcv_long;
    internal static delegate* unmanaged[Cdecl]<void*, int> _idcv_num;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _idcv_object;
    internal static delegate* unmanaged[Cdecl]<void*, int> _idcv_string;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int> _ieee2cpu;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ushort, int> _ieee_realcvt;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, ulong, void*, byte*, void> _import_module;
    internal static delegate* unmanaged[Cdecl]<byte> _indexer_is_enabled;
    internal static delegate* unmanaged[Cdecl]<nuint, QString*, void*, void*> _indexer_match;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, void*> _indexer_match_all;
    internal static delegate* unmanaged[Cdecl]<int, byte**, int*, int> _init_database;
    internal static delegate* unmanaged[Cdecl]<int, void> _init_plugins;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int, void> _insn_add_cref;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int, void> _insn_add_dref;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int, ulong> _insn_add_off_drefs;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte, byte> _insn_create_op_data;
    internal static delegate* unmanaged[Cdecl]<void*, void*, long, int, byte> _insn_create_stkvar;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _insn_get_next_byte;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _insn_get_next_dword;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _insn_get_next_qword;
    internal static delegate* unmanaged[Cdecl]<void*, ushort> _insn_get_next_word;
    internal static delegate* unmanaged[Cdecl]<void*, int> _install_custom_argloc;
    internal static delegate* unmanaged[Cdecl]<void*, nint> _install_extlang;
    internal static delegate* unmanaged[Cdecl]<nuint, void*, void*, byte> _install_user_defined_prefix;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, int, int> _internal_register_place_class;
    internal static delegate* unmanaged[Cdecl]<int, void> _interr;
    internal static delegate* unmanaged[Cdecl]<void> _invalidate_dbgmem_config;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _invalidate_dbgmem_contents;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, void> _invalidate_regfinder_cache;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void> _invalidate_regfinder_xrefs_cache;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _invoke_plugin;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _is_align_insn;
    internal static delegate* unmanaged[Cdecl]<int, int, byte> _is_attached_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<byte> _is_auto_enabled;
    internal static delegate* unmanaged[Cdecl]<void*, byte, byte> _is_basic_block_end;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _is_bnot;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _is_c_keyword;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _is_call_insn;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_char;
    internal static delegate* unmanaged[Cdecl]<int, int> _is_control_tty;
    internal static delegate* unmanaged[Cdecl]<uint, byte> _is_cp_graphical;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_custfmt;
    internal static delegate* unmanaged[Cdecl]<byte> _is_cvt64;
    internal static delegate* unmanaged[Cdecl]<byte> _is_database_busy;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _is_database_ext;
    internal static delegate* unmanaged[Cdecl]<uint, byte> _is_database_flag;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_debugger_memory;
    internal static delegate* unmanaged[Cdecl]<byte> _is_debugger_on;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_defarg;
    internal static delegate* unmanaged[Cdecl]<byte> _is_diff_merge_mode;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, byte> _is_ea_tryblks;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_enum;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_fltnum;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_forced_operand;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _is_func_locked;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_func_locked_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_function_entry;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_function_tail;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _is_ident;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_in_nlist;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _is_indirect_jump_insn;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _is_invsign;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_loaded;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_lzero;
    internal static delegate* unmanaged[Cdecl]<byte> _is_main_thread;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_manual;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_manual_insn;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_mapped;
    internal static delegate* unmanaged[Cdecl]<byte> _is_miniidb;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, ulong, ulong, byte> _is_name_defined_locally;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, ulong, ulong, byte> _is_name_defined_locally_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_numop;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_numop0;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_numop1;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_off;
    internal static delegate* unmanaged[Cdecl]<byte*, uint*, byte> _is_ordinal_name;
    internal static delegate* unmanaged[Cdecl]<byte, ulong, byte> _is_problem_present;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_public_name;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_refresh_requested;
    internal static delegate* unmanaged[Cdecl]<void*, byte, byte> _is_ret_insn;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _is_same_fchunk;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _is_same_segment;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_seg;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _is_segm_locked;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_segment_locked;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_spec_ea;
    internal static delegate* unmanaged[Cdecl]<byte, byte> _is_spec_segm;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_special_frame_member;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_stkvar;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _is_stroff;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _is_suspop;
    internal static delegate* unmanaged[Cdecl]<byte> _is_trusted_idb;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte> _is_type_choosable;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _is_uname;
    internal static delegate* unmanaged[Cdecl]<uint, int, void*, byte> _is_valid_cp;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _is_valid_typename;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _is_valid_utf8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, ulong*, int> _is_varsize_item;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _is_weak_name;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte, void> _iterate_func_chunks_ea;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _itext;
    internal static delegate* unmanaged[Cdecl]<void*, void> _jvalue_t_clear;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _jvalue_t_copy;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _l_compare;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int> _l_compare2;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _l_equals;
    internal static delegate* unmanaged[Cdecl]<void*, byte*> _last_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, void*> _launch_process;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _lcred_process_switch;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _leading_zero_important;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, int, byte, int> _lex_define_macro;
    internal static delegate* unmanaged[Cdecl]<void*, int*, byte**, int, byte*> _lex_get_file_line;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int*, int> _lex_get_token;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _lex_init_file;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*, int> _lex_init_string;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte*> _lex_print_token;
    internal static delegate* unmanaged[Cdecl]<void*, int, int> _lex_set_options;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void> _lex_term_file;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _lex_undefine_macro;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, int> _lexcompare_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _linearray_t_beginning;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _linearray_t_copy_from;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _linearray_t_ctr;
    internal static delegate* unmanaged[Cdecl]<void*, QString*> _linearray_t_down;
    internal static delegate* unmanaged[Cdecl]<void*, void> _linearray_t_dtr;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _linearray_t_ending;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _linearray_t_set_place;
    internal static delegate* unmanaged[Cdecl]<void*, QString*> _linearray_t_up;
    internal static delegate* unmanaged[Cdecl]<byte*, int, byte**, long> _llong_scan;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, byte> _load_core_module;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _load_dirtree;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _load_ids_module;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, byte*, ushort, void*, byte> _load_nonbinary_file;
    internal static delegate* unmanaged[Cdecl]<byte*, QString*, byte*, void*> _load_til;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, QString*, void*> _load_til_header;
    internal static delegate* unmanaged[Cdecl]<void*, byte**, byte*, void*, byte> _lochist_entry_t_deserialize;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _lochist_entry_t_serialize;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte, byte> _lochist_t_back;
    internal static delegate* unmanaged[Cdecl]<void*, void> _lochist_t_clear;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _lochist_t_current_index;
    internal static delegate* unmanaged[Cdecl]<void*, void> _lochist_t_deregister_live;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte, byte> _lochist_t_fwd;
    internal static delegate* unmanaged[Cdecl]<void*, void*, uint, byte> _lochist_t_get;
    internal static delegate* unmanaged[Cdecl]<void*, void*> _lochist_t_get_current;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*, void*, uint, byte> _lochist_t_init;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void*, void> _lochist_t_jump;
    internal static delegate* unmanaged[Cdecl]<void*, void> _lochist_t_register_live;
    internal static delegate* unmanaged[Cdecl]<void*, void> _lochist_t_save;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte, byte, byte> _lochist_t_seek;
    internal static delegate* unmanaged[Cdecl]<void*, uint, void*, void> _lochist_t_set;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _lochist_t_size;
    internal static delegate* unmanaged[Cdecl]<void> _lock_dbgmem_config;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void> _lock_func_range;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _lock_func_range_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void> _lock_segm;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _lock_segment_by_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _log2ceil;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _log2floor;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, void*> _lookup_loc_converter2;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, byte*, void*, int> _lower_type;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, void> _lread;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, byte, int> _lreadbytes;
    internal static delegate* unmanaged[Cdecl]<void*, void*> _make_linput;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _make_name_auto;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _make_name_non_public;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _make_name_non_weak;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _make_name_public;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _make_name_user;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _make_name_weak;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte*, void> _make_script_ns;
    internal static delegate* unmanaged[Cdecl]<byte, byte> _make_signatures;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, ulong> _map_code_ea;
    internal static delegate* unmanaged[Cdecl]<void*, int, int, void> _mark_switch_insns_jpt;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _match_jpt;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, int> _memicmp;
    internal static delegate* unmanaged[Cdecl]<void*, nint, void*> _memrev;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _move_idcv;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _move_privrange;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int> _move_segm;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _move_segm_start;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _move_segm_strerror;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, int> _move_segment;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte*, ulong, byte> _name_requires_qualifier;
    internal static delegate* unmanaged[Cdecl]<void*, byte**, byte*, void*, byte> _navstack_entry_t_deserialize;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _navstack_entry_t_serialize;
    internal static delegate* unmanaged[Cdecl]<void*, void> _navstack_t_deregister_live;
    internal static delegate* unmanaged[Cdecl]<void*, void> _navstack_t_dump;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _navstack_t_get_all_current;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, byte> _navstack_t_get_current;
    internal static delegate* unmanaged[Cdecl]<void*, void*, uint, byte> _navstack_t_get_stack_entry;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, uint, byte> _navstack_t_init;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, byte, byte> _navstack_t_perform_move;
    internal static delegate* unmanaged[Cdecl]<void*, void> _navstack_t_register_live;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte, void> _navstack_t_set_current;
    internal static delegate* unmanaged[Cdecl]<void*, uint, void*, void> _navstack_t_set_stack_entry;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _navstack_t_stack_clear;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _navstack_t_stack_index;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void*, void> _navstack_t_stack_jump;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte, uint, byte, byte> _navstack_t_stack_nav;
    internal static delegate* unmanaged[Cdecl]<void*, void*, uint, byte, byte, byte> _navstack_t_stack_seek;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _navstack_t_stack_size;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _nbits;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, void*, void> _netnode_altadjust2;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, int, nuint> _netnode_altshift;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, ulong> _netnode_altval;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int, ulong> _netnode_altval_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, int, nuint> _netnode_blobshift;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, nuint> _netnode_blobsize;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, int, nuint> _netnode_charshift;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _netnode_charval;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int, byte> _netnode_charval_idx8;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, nuint, byte, byte> _netnode_check;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte, nuint> _netnode_copy;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, int> _netnode_delblob;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _netnode_delvalue;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _netnode_end;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _netnode_exist;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, nint> _netnode_get_name;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint*, ulong, int, void*> _netnode_getblob;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, byte> _netnode_hashdel;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, nuint, int, nint> _netnode_hashfirst;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, nuint, int, nint> _netnode_hashlast;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte*, nuint, int, nint> _netnode_hashnext;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte*, nuint, int, nint> _netnode_hashprev;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, void*, nuint, int, byte> _netnode_hashset;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte*, nuint, int, nint> _netnode_hashstr;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, void*, nuint, int, nint> _netnode_hashval;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, ulong> _netnode_hashval_long;
    internal static delegate* unmanaged[Cdecl]<byte> _netnode_inited;
    internal static delegate* unmanaged[Cdecl]<byte> _netnode_is_available;
    internal static delegate* unmanaged[Cdecl]<void*, void> _netnode_kill;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, ulong> _netnode_lower_bound;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int, ulong> _netnode_lower_bound_idx8;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _netnode_next;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _netnode_prev;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint, ulong, int, nint> _netnode_qgetblob;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, int, nint> _netnode_qhashfirst;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, int, nint> _netnode_qhashlast;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, byte*, int, nint> _netnode_qhashnext;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, byte*, int, nint> _netnode_qhashprev;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, byte*, int, nint> _netnode_qhashstr;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, ulong, int, nint> _netnode_qsupstr;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, byte, int, nint> _netnode_qsupstr_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, QString*, nint> _netnode_qvalstr;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, nuint, byte> _netnode_rename;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint, byte> _netnode_set;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint, ulong, int, byte> _netnode_setblob;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _netnode_start;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _netnode_supdel;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _netnode_supdel_all;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int, byte> _netnode_supdel_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, int, int> _netnode_supdel_range;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, int, int> _netnode_supdel_range_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _netnode_supfirst;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _netnode_supfirst_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _netnode_suplast;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _netnode_suplast_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, ulong> _netnode_supnext;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int, ulong> _netnode_supnext_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, ulong> _netnode_supprev;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, int, ulong> _netnode_supprev_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, nuint, int, byte> _netnode_supset;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void*, nuint, int, byte> _netnode_supset_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, ulong, int, nuint> _netnode_supshift;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte*, nuint, int, nint> _netnode_supstr;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, byte*, nuint, int, nint> _netnode_supstr_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, nuint, int, nint> _netnode_supval;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void*, nuint, int, nint> _netnode_supval_idx8;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint, nint> _netnode_valobj;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, nuint, nint> _netnode_valstr;
    internal static delegate* unmanaged[Cdecl]<byte, byte*, nuint, int, void*> _new_packet;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, void*> _new_til;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _next_addr;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _next_chunk;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _next_head;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*> _next_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, byte*> _next_named_type;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _next_not_tail;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, void*, ulong> _next_that;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _next_visea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _node2ea;
    internal static delegate* unmanaged[Cdecl]<void*, void*> _node_iterator_goup;
    internal static delegate* unmanaged[Cdecl]<void*, byte, ulong, void> _notify_dirtree;
    internal static delegate* unmanaged[Cdecl]<ulong> _num_flag;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, ulong, int, ulong, int, int, nuint> _numop2str;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _op_adds_xrefs;
    internal static delegate* unmanaged[Cdecl]<void*, int, long, ulong, byte> _op_based_stroff;
    internal static delegate* unmanaged[Cdecl]<ulong, int, int, byte> _op_custfmt;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, byte, byte> _op_enum;
    internal static delegate* unmanaged[Cdecl]<ulong, int, uint, ulong, ulong, long, byte> _op_offset;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void*, byte> _op_offset_ex;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _op_seg;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _op_stkvar;
    internal static delegate* unmanaged[Cdecl]<void*, int, ulong*, int, long, byte> _op_stroff;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _openM;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _openR;
    internal static delegate* unmanaged[Cdecl]<byte*, void*> _openRT;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte*, int> _open_database;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, void*> _open_linput;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, byte> _optimize_argloc;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, uint, byte*> _pack_dd;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, ulong, byte*> _pack_dq;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, nuint, byte*> _pack_ds;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, ushort, byte*> _pack_dw;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, void*, void*, int, int> _pack_idcobj_to_bv;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, ulong, int, int> _pack_idcobj_to_idb;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte*, int, int, QString*, byte> _parse_binpat_str;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, int, nuint> _parse_command_line;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, byte> _parse_config_value;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _parse_dbgopts;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, QString*, void*, byte*, int, byte> _parse_decl;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*, int, int> _parse_decls;
    internal static delegate* unmanaged[Cdecl]<int, void*, byte*, byte, int> _parse_decls_for_srclang;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, byte*, byte, int> _parse_decls_with_parser;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, byte*, int, int> _parse_decls_with_parser_ext;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int> _parse_json;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, QString*, int> _parse_json_file;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, QString*, byte*, int> _parse_json_string;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _parse_plugin_options;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, byte> _parse_pretty_size;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _parse_reg_name;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, uint, byte> _parse_timestamp;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _patch_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint, void> _patch_bytes;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _patch_dword;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte> _patch_fixup_value;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _patch_qword;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _patch_word;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong> _peek_auto_queue;
    internal static delegate* unmanaged[Cdecl]<byte> _perform_redo;
    internal static delegate* unmanaged[Cdecl]<byte> _perform_undo;
    internal static delegate* unmanaged[Cdecl]<int*, int*, void*, QString*, void*> _pipe_process;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte, int> _plan_and_wait;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _plan_to_apply_idasgn;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, void> _plugin_name_from_path_or_name;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, byte, byte> _plugin_option_t_get_bool;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, ulong, nuint> _pretty_print_size;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _prev_addr;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _prev_chunk;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong> _prev_head;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*> _prev_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _prev_not_tail;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void*, void*, ulong> _prev_that;
    internal static delegate* unmanaged[Cdecl]<uint*, byte**, byte*, byte> _prev_utf8_char;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _prev_visea;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*, int, int, nuint> _print_argloc;
    internal static delegate* unmanaged[Cdecl]<void*, void*, TypeInfo*, void*, int> _print_cdata;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, uint, int> _print_decls;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*, int, byte> _print_fpval;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, byte*, int, byte> _print_idcv;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte> _print_insn_mnem;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, int, void*, byte> _print_operand;
    internal static delegate* unmanaged[Cdecl]<QString*, int, QString*, int, byte> _print_strlit_type;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, int, int, TypeInfo*, byte*, byte*, byte> _print_tinfo;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, int, byte> _print_type;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, QString*, ushort*, byte*, void*, QString*, int> _process_archive;
    internal static delegate* unmanaged[Cdecl]<byte*, int, void> _process_config_directive;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _put_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, nuint, void> _put_bytes;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, byte> _put_dbg_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _put_dword;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _put_qword;
    internal static delegate* unmanaged[Cdecl]<byte*, uint, nint> _put_utf8_char;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _put_word;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int> _qaccess;
    internal static delegate* unmanaged[Cdecl]<nuint, void*> _qalloc;
    internal static delegate* unmanaged[Cdecl]<nuint, void*> _qalloc_or_throw;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _qbasename;
    internal static delegate* unmanaged[Cdecl]<nuint, nuint, void*> _qcalloc;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _qchdir;
    internal static delegate* unmanaged[Cdecl]<int, ulong, int> _qchsize;
    internal static delegate* unmanaged[Cdecl]<QString*, byte, uint, nint> _qcleanline;
    internal static delegate* unmanaged[Cdecl]<int, int> _qclose;
    internal static delegate* unmanaged[Cdecl]<void> _qcontrol_tty;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int> _qcreate;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, int, byte> _qctime;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, int, byte> _qctime_utc;
    internal static delegate* unmanaged[Cdecl]<void> _qdetach_tty;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte> _qdirname;
    internal static delegate* unmanaged[Cdecl]<int, int> _qdup;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _qerrstr;
    internal static delegate* unmanaged[Cdecl]<int, void> _qexit;
    internal static delegate* unmanaged[Cdecl]<void*, int> _qfclose;
    internal static delegate* unmanaged[Cdecl]<void*, int> _qfgetc;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*, byte*> _qfgets;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _qfileexist;
    internal static delegate* unmanaged[Cdecl]<int, ulong> _qfilelength;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _qfilesize;
    internal static delegate* unmanaged[Cdecl]<void*, void> _qfindclose;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, int, int> _qfindfirst;
    internal static delegate* unmanaged[Cdecl]<void*, int> _qfindnext;
    internal static delegate* unmanaged[Cdecl]<void*, int> _qflush;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, void*> _qfopen;
    internal static delegate* unmanaged[Cdecl]<int, void*, int> _qfputc;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, int> _qfputs;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, nint> _qfread;
    internal static delegate* unmanaged[Cdecl]<void*, void> _qfree;
    internal static delegate* unmanaged[Cdecl]<void*, ulong> _qfsize;
    internal static delegate* unmanaged[Cdecl]<int, void*, int> _qfstat;
    internal static delegate* unmanaged[Cdecl]<int, int> _qfsync;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, nint> _qfwrite;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void> _qgetcwd;
    internal static delegate* unmanaged[Cdecl]<QString*, byte> _qgethostname;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, nint> _qgetline;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*> _qgets;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, ushort, ushort, byte> _qhost2addr_;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _qisabspath;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _qisdir;
    internal static delegate* unmanaged[Cdecl]<void*, void*> _qlfile;
    internal static delegate* unmanaged[Cdecl]<void*, int> _qlgetc;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*, byte*> _qlgets;
    internal static delegate* unmanaged[Cdecl]<void*, long, byte*, nuint, byte*> _qlgetz;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, nint> _qlread;
    internal static delegate* unmanaged[Cdecl]<void*, long> _qlsize;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte*> _qmake_full_path;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte*, byte*> _qmakefile;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int> _qmkdir;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, uint, int> _qmove;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int> _qopen;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int, int> _qopen_shared;
    internal static delegate* unmanaged[Cdecl]<int, int> _qpipe_close;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, nint> _qpipe_read;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, byte> _qpipe_read_n;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, nint> _qpipe_write;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, int> _qread;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, void*> _qrealloc;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, void*> _qrealloc_or_throw;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, int> _qregcomp;
    internal static delegate* unmanaged[Cdecl]<int, void*, byte*, nuint, nuint> _qregerror;
    internal static delegate* unmanaged[Cdecl]<void*, void> _qregfree;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, int> _qrename;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _qrmdir;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte> _qsetenv;
    internal static delegate* unmanaged[Cdecl]<int, void> _qsleep;
    internal static delegate* unmanaged[Cdecl]<byte*, byte**, byte**, byte*> _qsplitfile;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, int> _qstat;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte*> _qstpncpy;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, void> _qstr2user;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _qstrcmp;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _qstrdup;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _qstrerror;
    internal static delegate* unmanaged[Cdecl]<void*, nuint> _qstrlen;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _qstrlwr;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte*> _qstrncat;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, int> _qstrncmp;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte*> _qstrncpy;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte**, byte*> _qstrtok;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _qstrupr;
    internal static delegate* unmanaged[Cdecl]<ulong> _qtime64;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*> _qtmpdir;
    internal static delegate* unmanaged[Cdecl]<void*> _qtmpfile;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*> _qtmpnam;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _qtouchfile;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _qunlink;
    internal static delegate* unmanaged[Cdecl]<QString*, byte> _quote_cmdline_arg;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint> _qustrlen;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte> _qustrncpy;
    internal static delegate* unmanaged[Cdecl]<void*, void*, nuint, nuint, void*> _qvector_reserve;
    internal static delegate* unmanaged[Cdecl]<int*, int*, int, uint, int, int> _qwait_for_handles;
    internal static delegate* unmanaged[Cdecl]<int*, int, int, int, int> _qwait_timed;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, int> _qwrite;
    internal static delegate* unmanaged[Cdecl]<byte*, ushort*, int, int> _r50_to_asc;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, nuint, nuint> _range_t_print;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_add;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_add2;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_contains;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void*> _rangeset_t_find_range;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte, byte> _rangeset_t_has_common;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_has_common2;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_intersect;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _rangeset_t_next_addr;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _rangeset_t_next_range;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _rangeset_t_prev_addr;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _rangeset_t_prev_range;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, nuint, nuint> _rangeset_t_print;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_sub;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte> _rangeset_t_sub2;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _rangeset_t_swap;
    internal static delegate* unmanaged[Cdecl]<int, ushort*, byte, int> _read2bytes;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, byte*, void*, nint> _read_ioports;
    internal static delegate* unmanaged[Cdecl]<void*, void> _read_regargs;
    internal static delegate* unmanaged[Cdecl]<ulong*, long*, ulong, int, int> _read_struc_path;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, ulong> _read_tinfo_bitfield_value;
    internal static delegate* unmanaged[Cdecl]<int, uint*, int, byte, int> _readbytes;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, void*, uint, void> _realtoasc;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _reanalyze_callers;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, byte, void> _reanalyze_function;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, byte, void> _reanalyze_function_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _reanalyze_noret_flag;
    internal static delegate* unmanaged[Cdecl]<long, int, int> _rebase_program;
    internal static delegate* unmanaged[Cdecl]<void> _rebuild_nlist;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _recalc_func_spd_for_basic_block;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _recalc_spd;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _recalc_spd_for_basic_block;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, void*, nuint, byte*, int, byte> _reg_bin_op;
    internal static delegate* unmanaged[Cdecl]<int*, byte*, byte*, byte> _reg_data_type;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte> _reg_delete;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _reg_delete_subkey;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _reg_delete_tree;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte> _reg_exists;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, int, byte> _reg_finder94_find_reg_value_info;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void*, ulong, void> _reg_finder94_make_rfop;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void*, ulong, ulong, int, byte> _reg_finder_calc_op_addr;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _reg_finder_can_resolve_mem;
    internal static delegate* unmanaged[Cdecl]<void*, void> _reg_finder_ctr;
    internal static delegate* unmanaged[Cdecl]<void*, void> _reg_finder_dtr;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, void*, void*, int, byte, int, byte, void*, ulong, ulong, void> _reg_finder_emulate_binary_op_shifted;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int, byte, void*, byte> _reg_finder_emulate_mem_read;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, int, void*, ulong, ulong, void> _reg_finder_emulate_unary_op;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, int, void> _reg_finder_invalidate_cache;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, void> _reg_finder_invalidate_xrefs_cache;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void*, void> _reg_finder_make_rfop;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, int, byte*, int> _reg_int_op;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _reg_read_strlist;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, byte*, byte> _reg_str_get;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, void> _reg_str_set;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte, byte> _reg_subkey_children;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _reg_subkey_exists;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte*, byte, void> _reg_update_strlist;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, void*, void> _reg_value_base_dstr;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _reg_value_base_vals_union;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, int, void*, void> _reg_value_def_dstr;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, void*, void> _reg_value_info_dstr;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _reg_value_info_vals_union;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _reg_write_strlist;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _regarg_t__compare;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte, int> _regex_match;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _register_custom_callcnv;
    internal static delegate* unmanaged[Cdecl]<void*, int> _register_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<void*, int> _register_custom_data_type;
    internal static delegate* unmanaged[Cdecl]<void*, ushort> _register_custom_fixup;
    internal static delegate* unmanaged[Cdecl]<void*, int> _register_custom_refinfo;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, void*, void> _register_loc_converter2;
    internal static delegate* unmanaged[Cdecl]<int, void*, void*, byte> _register_post_event_visitor;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int> _regvar_t__compare;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte> _reload_file;
    internal static delegate* unmanaged[Cdecl]<void*, int, long, byte, void> _reloc_value;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte, byte> _relocate_relobj;
    internal static delegate* unmanaged[Cdecl]<byte, ulong, byte*, void> _remember_problem;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte> _remove_abi_opts;
    internal static delegate* unmanaged[Cdecl]<int, byte> _remove_custom_argloc;
    internal static delegate* unmanaged[Cdecl]<void*, void> _remove_event_listener;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _remove_extlang;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _remove_func_tail;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _remove_func_tail_ea;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, byte**, void*, byte> _remove_tinfo_pointer;
    internal static delegate* unmanaged[Cdecl]<int, byte*, byte> _rename_encoding;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, byte> _rename_entry;
    internal static delegate* unmanaged[Cdecl]<ulong, nint, byte*, int> _rename_func_regvar;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, int> _rename_regvar;
    internal static delegate* unmanaged[Cdecl]<void> _reorder_dummy_names;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, int> _replace_ordinal_typerefs;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, byte> _replace_tabs;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _request_refresh;
    internal static delegate* unmanaged[Cdecl]<void*, void> _reset_dirtree;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, byte> _resolve_field_path;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*> _resolve_typedef;
    internal static delegate* unmanaged[Cdecl]<int, void*> _retrieve_custom_argloc;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _revert_byte;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _revert_ida_decisions;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _revert_metadata;
    internal static delegate* unmanaged[Cdecl]<ulong, int, nuint, nuint, ulong> _rotate_left;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, byte> _run_plugin;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, byte> _same_value_jpt;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte> _sanitize_file_name;
    internal static delegate* unmanaged[Cdecl]<byte*, uint, void*, void*, byte> _save_database;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _save_dirtree;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, nuint, byte*, int, int> _save_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, uint> _score_metadata;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, uint> _score_tinfo;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, int*, byte*, int, int> _search;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte, byte> _search_path;
    internal static delegate* unmanaged[Cdecl]<void*, long, long> _segm_adjust_diff;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong> _segm_adjust_ea;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, byte> _segment_info_t__visible_name;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _segtype;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _sel2para;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _select_extlang;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _select_parser_by_name;
    internal static delegate* unmanaged[Cdecl]<int, byte> _select_parser_by_srclang;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _serialize_dynamic_register_set;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _serialize_insn;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, uint, byte> _serialize_json;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, TypeInfo*, int, byte> _serialize_tinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, void> _set_abits;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, void> _set_aflags;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _set_array_parameters;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, long, byte> _set_auto_spd;
    internal static delegate* unmanaged[Cdecl]<int, int> _set_auto_state;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte, byte> _set_cmt;
    internal static delegate* unmanaged[Cdecl]<void*, int, byte*, byte> _set_compiler;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte> _set_compiler_string;
    internal static delegate* unmanaged[Cdecl]<int, uint, uint, byte, void> _set_cp_validity;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _set_custom_data_type_ids;
    internal static delegate* unmanaged[Cdecl]<uint, byte, void> _set_database_flag;
    internal static delegate* unmanaged[Cdecl]<void*, int, void> _set_debug_event_code;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte> _set_debug_name;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte**, int, int> _set_debug_names;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _set_default_dataseg;
    internal static delegate* unmanaged[Cdecl]<int, int, byte> _set_default_encoding_idx;
    internal static delegate* unmanaged[Cdecl]<void*, int, ulong, byte> _set_default_sreg_value;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, byte> _set_default_sreg_value_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _set_dummy_name;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, byte> _set_entry_forwarder;
    internal static delegate* unmanaged[Cdecl]<int, nuint, void> _set_error_data;
    internal static delegate* unmanaged[Cdecl]<int, byte*, void> _set_error_string;
    internal static delegate* unmanaged[Cdecl]<byte*, nuint, byte*, byte*, byte*> _set_file_ext;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _set_fixup;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte*, byte> _set_forced_operand;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, TypeInfo*, void*, uint, byte> _set_frame_member_type;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, TypeInfo*, void*, uint, byte> _set_frame_member_type_ea;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ushort, ulong, byte> _set_frame_size;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ushort, ulong, byte> _set_frame_size_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, long, byte> _set_func_auto_spd;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte, byte> _set_func_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte, byte> _set_func_cmt_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _set_func_end;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _set_func_entry_info;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte, byte> _set_func_flag;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _set_func_flags;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _set_func_name_if_jumpfunc;
    internal static delegate* unmanaged[Cdecl]<ulong, nint, byte*, int> _set_func_regvar_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, nint, void*, int> _set_func_regvar_range;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int> _set_func_start;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte> _set_function_name_if_jumpfunc;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int> _set_group_selector;
    internal static delegate* unmanaged[Cdecl]<byte*, byte, byte> _set_header_path;
    internal static delegate* unmanaged[Cdecl]<int, int> _set_ida_state;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*> _set_idc_dtor;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*> _set_idc_getattr;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _set_idc_method;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*> _set_idc_setattr;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void*, byte, int> _set_idcv_attr;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, ulong, void*, int, int> _set_idcv_slice;
    internal static delegate* unmanaged[Cdecl]<ulong, byte> _set_immd;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte*, void> _set_import_name;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, void> _set_import_ordinal;
    internal static delegate* unmanaged[Cdecl]<byte, byte> _set_interr_throws;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, void> _set_item_color;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _set_lzero;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, void> _set_manual_insn;
    internal static delegate* unmanaged[Cdecl]<int*, void*, void*> _set_module_data;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void*, void*, byte, byte, byte> _set_moved_jpt;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, byte> _set_name;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void*, uint, void> _set_node_info;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, byte> _set_noret_insn;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _set_notcode;
    internal static delegate* unmanaged[Cdecl]<ulong, int, TypeInfo*, byte> _set_op_tinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _set_op_type;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, void*, byte, byte> _set_opinfo;
    internal static delegate* unmanaged[Cdecl]<int, byte> _set_outfile_encoding_idx;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, int> _set_parser_argv;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, byte> _set_parser_option;
    internal static delegate* unmanaged[Cdecl]<int, byte*, void> _set_path;
    internal static delegate* unmanaged[Cdecl]<byte*, int, byte> _set_processor_type;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte, byte> _set_purged;
    internal static delegate* unmanaged[Cdecl]<int, int> _set_qerrno;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte, ulong, ulong, long, byte> _set_refinfo;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void*, byte> _set_refinfo_ex;
    internal static delegate* unmanaged[Cdecl]<byte*, byte> _set_registry_name;
    internal static delegate* unmanaged[Cdecl]<void*, void*, byte*, int> _set_regvar_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _set_screen_ea;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, byte> _set_segm_addressing;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _set_segm_base;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, int> _set_segm_class;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _set_segm_end;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, int> _set_segm_name;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, byte> _set_segm_start;
    internal static delegate* unmanaged[Cdecl]<ulong, nuint, byte> _set_segment_addressing;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _set_segment_base_ea;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, int> _set_segment_class;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte, void> _set_segment_cmt;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, byte, void> _set_segment_cmt_by_ea;
    internal static delegate* unmanaged[Cdecl]<void*, int, byte> _set_segment_info;
    internal static delegate* unmanaged[Cdecl]<ulong, byte*, int, int> _set_segment_name;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, byte> _set_segment_translations;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int> _set_selector;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, void> _set_source_linnum;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, int, ulong, void> _set_sreg_at_next_code;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, void> _set_str_type;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _set_switch_info;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _set_tail_owner;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _set_tail_owner_ea;
    internal static delegate* unmanaged[Cdecl]<int, byte> _set_target_assembler;
    internal static delegate* unmanaged[Cdecl]<ulong, TypeInfo*, byte> _set_tinfo;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, byte, byte> _set_tinfo_attr;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, byte> _set_tinfo_attrs;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, int, nuint, nuint> _set_tinfo_property;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, int, nuint, nuint, nuint, nuint, nuint> _set_tinfo_property4;
    internal static delegate* unmanaged[Cdecl]<void*, uint, uint, byte> _set_type_alias;
    internal static delegate* unmanaged[Cdecl]<void*, uint, byte, void> _set_type_choosable;
    internal static delegate* unmanaged[Cdecl]<uint, ulong, byte> _set_vftable_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void> _set_visible_func;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _set_visible_func_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte, void> _set_visible_segm;
    internal static delegate* unmanaged[Cdecl]<ulong, byte, void> _set_visible_segment;
    internal static delegate* unmanaged[Cdecl]<ulong, void*, void> _set_xrefpos;
    internal static delegate* unmanaged[Cdecl]<int, nint, byte> _setinf;
    internal static delegate* unmanaged[Cdecl]<int, void*, nuint, byte> _setinf_buf;
    internal static delegate* unmanaged[Cdecl]<int, uint, byte, byte> _setinf_flag;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _setup_lowcnd_regfuncs;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _setup_selector;
    internal static delegate* unmanaged[Cdecl]<ulong, int, void> _show_auto;
    internal static delegate* unmanaged[Cdecl]<ulong, void> _show_name;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _skip_spaces;
    internal static delegate* unmanaged[Cdecl]<byte**, nuint, nuint> _skip_utf8;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, long> _soff_to_fpoff_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _sort_til;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong, byte, byte, byte> _split_sreg_range;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _std_out_segm_footer;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, void> _std_out_segment_footer;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, ulong, nuint> _stoa;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte*, byte> _store_til;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, ulong, byte> _str2ea;
    internal static delegate* unmanaged[Cdecl]<ulong*, byte*, ulong, int, byte> _str2ea_ex;
    internal static delegate* unmanaged[Cdecl]<byte*, int> _str2reg;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte*> _str2user;
    internal static delegate* unmanaged[Cdecl]<void*, nuint, int, byte*> _strarray;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _strdiff_t_serialize;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, byte*> _stristr;
    internal static delegate* unmanaged[Cdecl]<byte*, int, int, byte*> _strrpl;
    internal static delegate* unmanaged[Cdecl]<void*, void> _swap128;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _swap64;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _swap_idcvs;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, void> _swap_value;
    internal static delegate* unmanaged[Cdecl]<nuint, void*> _switch_dbctx;
    internal static delegate* unmanaged[Cdecl]<void> _switch_to_rust;
    internal static delegate* unmanaged[Cdecl]<QString*, ulong, byte, void> _tag_addr;
    internal static delegate* unmanaged[Cdecl]<byte*, int, byte*> _tag_advance;
    internal static delegate* unmanaged[Cdecl]<byte*, ulong> _tag_get_addr;
    internal static delegate* unmanaged[Cdecl]<QString*, byte*, int, nint> _tag_remove;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _tag_skipcode;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _tag_skipcodes;
    internal static delegate* unmanaged[Cdecl]<byte*, nint> _tag_strlen;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, ulong> _tagged_line_section_t_get_addr;
    internal static delegate* unmanaged[Cdecl]<int, byte> _take_memory_snapshot;
    internal static delegate* unmanaged[Cdecl]<void> _term_database;
    internal static delegate* unmanaged[Cdecl]<int, void> _term_plugins;
    internal static delegate* unmanaged[Cdecl]<void*, int> _term_process;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int> _throw_idc_exception;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _tinfo_errstr;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, void*, byte> _tinfo_get_func_frame;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, byte> _tinfo_get_func_frame_ea;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, TypeInfo*, ulong, nuint*, ulong*, byte, void> _tinfo_get_innermost_udm;
    internal static delegate* unmanaged[Cdecl]<QString*, TypeInfo*, byte> _tinfo_t__build_anon_type_name;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _toggle_bnot;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte> _toggle_sign;
    internal static delegate* unmanaged[Cdecl]<void*, void*, ulong, byte> _track_value_until_address_jpt;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*> _trim;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte, void> _trim_jtable;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _try_to_add_libfunc;
    internal static delegate* unmanaged[Cdecl]<void*, void> _txtdiff_t_diff_mod;
    internal static delegate* unmanaged[Cdecl]<void*, void*, void> _txtdiff_t_serialize;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, byte> _udm_t__compare_with;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _udt_type_data_t__deduplicate_members;
    internal static delegate* unmanaged[Cdecl]<void*, void*, int, nint> _udt_type_data_t__find_member;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, nint> _udt_type_data_t__get_best_fit_member;
    internal static delegate* unmanaged[Cdecl]<int, void*, byte> _unhook_event_listener;
    internal static delegate* unmanaged[Cdecl]<int, void*, void*, int> _unhook_from_notification_point;
    internal static delegate* unmanaged[Cdecl]<void> _unlock_dbgmem_config;
    internal static delegate* unmanaged[Cdecl]<void*, void> _unmake_linput;
    internal static delegate* unmanaged[Cdecl]<byte**, byte*, uint> _unpack_dd;
    internal static delegate* unmanaged[Cdecl]<byte**, byte*, ulong> _unpack_dq;
    internal static delegate* unmanaged[Cdecl]<byte**, byte*, byte, byte*> _unpack_ds;
    internal static delegate* unmanaged[Cdecl]<byte**, byte*, ushort> _unpack_dw;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, void*, int, int> _unpack_idcobj_from_bv;
    internal static delegate* unmanaged[Cdecl]<void*, TypeInfo*, ulong, void*, int, int> _unpack_idcobj_from_idb;
    internal static delegate* unmanaged[Cdecl]<void*, int, byte, byte**, byte*, byte> _unpack_xleb128;
    internal static delegate* unmanaged[Cdecl]<uint, byte> _unregister_custom_callcnv;
    internal static delegate* unmanaged[Cdecl]<int, byte> _unregister_custom_data_format;
    internal static delegate* unmanaged[Cdecl]<int, byte> _unregister_custom_data_type;
    internal static delegate* unmanaged[Cdecl]<ushort, byte> _unregister_custom_fixup;
    internal static delegate* unmanaged[Cdecl]<int, byte> _unregister_custom_refinfo;
    internal static delegate* unmanaged[Cdecl]<int, void*, byte> _unregister_post_event_visitor;
    internal static delegate* unmanaged[Cdecl]<ulong, uint, uint, void> _upd_abits;
    internal static delegate* unmanaged[Cdecl]<ulong, int, byte*, byte> _update_extra_cmt;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, byte> _update_fpd;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, byte> _update_fpd_ea;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _update_func;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _update_hidden_range;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _update_hidden_range_info;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _update_segm;
    internal static delegate* unmanaged[Cdecl]<byte*, void*, void*, int, byte> _update_snapshot_attributes;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong> _use_mapping;
    internal static delegate* unmanaged[Cdecl]<QString*, QString*, void> _user2qstr;
    internal static delegate* unmanaged[Cdecl]<byte*, byte*, nuint, byte*> _user2str;
    internal static delegate* unmanaged[Cdecl]<QString*, void*, int, byte> _utf16_utf8;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, int, byte> _utf8_utf16;
    internal static delegate* unmanaged[Cdecl]<uint, nuint> _validate_idb;
    internal static delegate* unmanaged[Cdecl]<byte, int> _validate_idb_names;
    internal static delegate* unmanaged[Cdecl]<QString*, int, int, byte> _validate_name;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, uint, void*, void*, byte> _value_repr_t__from_opinfo;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, byte, byte> _value_repr_t__parse_value_repr;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, byte, nuint> _value_repr_t__print_;
    internal static delegate* unmanaged[Cdecl]<void*, void*, QString*, uint, int> _vcred_ask_user;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, QString*, byte> _vcred_do_load_password;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, QString*, byte> _vcred_do_load_proxy_password;
    internal static delegate* unmanaged[Cdecl]<void*, void> _vcred_init;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _vcred_load_site;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, byte> _vcred_process_switch;
    internal static delegate* unmanaged[Cdecl]<byte> _vcred_reg_del_auto_connect;
    internal static delegate* unmanaged[Cdecl]<byte> _vcred_reg_del_store_info;
    internal static delegate* unmanaged[Cdecl]<byte, void> _vcred_reg_set_auto_connect;
    internal static delegate* unmanaged[Cdecl]<void*, byte*, void> _vcred_reg_set_site;
    internal static delegate* unmanaged[Cdecl]<byte, void> _vcred_reg_set_store_info;
    internal static delegate* unmanaged[Cdecl]<byte> _vcred_reg_should_auto_connect;
    internal static delegate* unmanaged[Cdecl]<byte> _vcred_reg_should_store_info;
    internal static delegate* unmanaged[Cdecl]<void*, QString*, byte> _vcred_write;
    internal static delegate* unmanaged[Cdecl]<void*, int, void*, int> _verify_argloc;
    internal static delegate* unmanaged[Cdecl]<ulong, int> _verify_tinfo;
    internal static delegate* unmanaged[Cdecl]<TypeInfo*, ulong, int, byte, void*, nint> _visit_edms;
    internal static delegate* unmanaged[Cdecl]<void*, ulong*, int, long*, byte, int> _visit_stroff_udms;
    internal static delegate* unmanaged[Cdecl]<void*, void*, TypeInfo*, byte*, byte*, int> _visit_subtypes;
    internal static delegate* unmanaged[Cdecl]<int, byte*> _winerr;
    internal static delegate* unmanaged[Cdecl]<ulong, int, ulong*, int, long, void> _write_struc_path;
    internal static delegate* unmanaged[Cdecl]<ulong, ulong, ulong, int, ulong> _write_tinfo_bitfield_value;
    internal static delegate* unmanaged[Cdecl]<int, uint, int, byte, int> _writebytes;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _xrefblk_t_first_from;
    internal static delegate* unmanaged[Cdecl]<void*, ulong, int, byte> _xrefblk_t_first_to;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _xrefblk_t_next_from;
    internal static delegate* unmanaged[Cdecl]<void*, byte> _xrefblk_t_next_to;
    internal static delegate* unmanaged[Cdecl]<byte, byte> _xrefchar;

    public static void @MD5Init(void* p0)
    {
        if ((nint)_MD5Init == 0) throw new System.EntryPointNotFoundException("'MD5Init' is not available for the loaded IDA SDK version.");
        _MD5Init(p0);
    }

    public static void @MD5Update(void* p0, void* p1, nuint p2)
    {
        if ((nint)_MD5Update == 0) throw new System.EntryPointNotFoundException("'MD5Update' is not available for the loaded IDA SDK version.");
        _MD5Update(p0, p1, p2);
    }

    public static byte @add_auto_stkpnt(void* p0, ulong p1, long p2)
    {
        if ((nint)_add_auto_stkpnt == 0) throw new System.EntryPointNotFoundException("'add_auto_stkpnt' is not available for the loaded IDA SDK version.");
        return _add_auto_stkpnt(p0, p1, p2);
    }

    public static int @add_base_tils(QString* p0, void* p1, byte* p2, byte* p3, byte p4)
    {
        if ((nint)_add_base_tils == 0) throw new System.EntryPointNotFoundException("'add_base_tils' is not available for the loaded IDA SDK version.");
        return _add_base_tils(p0, p1, p2, p3, p4);
    }

    public static void @add_byte(ulong p0, uint p1)
    {
        if ((nint)_add_byte == 0) throw new System.EntryPointNotFoundException("'add_byte' is not available for the loaded IDA SDK version.");
        _add_byte(p0, p1);
    }

    public static byte @add_cref(ulong p0, ulong p1, int p2)
    {
        if ((nint)_add_cref == 0) throw new System.EntryPointNotFoundException("'add_cref' is not available for the loaded IDA SDK version.");
        return _add_cref(p0, p1, p2);
    }

    public static byte @add_dref(ulong p0, ulong p1, int p2)
    {
        if ((nint)_add_dref == 0) throw new System.EntryPointNotFoundException("'add_dref' is not available for the loaded IDA SDK version.");
        return _add_dref(p0, p1, p2);
    }

    public static void @add_dword(ulong p0, ulong p1)
    {
        if ((nint)_add_dword == 0) throw new System.EntryPointNotFoundException("'add_dword' is not available for the loaded IDA SDK version.");
        _add_dword(p0, p1);
    }

    public static int @add_encoding(byte* p0)
    {
        if ((nint)_add_encoding == 0) throw new System.EntryPointNotFoundException("'add_encoding' is not available for the loaded IDA SDK version.");
        return _add_encoding(p0);
    }

    public static byte @add_entry(ulong p0, ulong p1, byte* p2, byte p3, int p4)
    {
        if ((nint)_add_entry == 0) throw new System.EntryPointNotFoundException("'add_entry' is not available for the loaded IDA SDK version.");
        return _add_entry(p0, p1, p2, p3, p4);
    }

    public static byte @add_frame(void* p0, long p1, ushort p2, ulong p3)
    {
        if ((nint)_add_frame == 0) throw new System.EntryPointNotFoundException("'add_frame' is not available for the loaded IDA SDK version.");
        return _add_frame(p0, p1, p2, p3);
    }

    public static byte @add_frame_ea(ulong p0, long p1, ushort p2, ulong p3)
    {
        if ((nint)_add_frame_ea == 0) throw new System.EntryPointNotFoundException("'add_frame_ea' is not available for the loaded IDA SDK version.");
        return _add_frame_ea(p0, p1, p2, p3);
    }

    public static byte @add_frame_member(void* p0, byte* p1, ulong p2, TypeInfo* p3, void* p4, uint p5)
    {
        if ((nint)_add_frame_member == 0) throw new System.EntryPointNotFoundException("'add_frame_member' is not available for the loaded IDA SDK version.");
        return _add_frame_member(p0, p1, p2, p3, p4, p5);
    }

    public static byte @add_frame_member_ea(ulong p0, byte* p1, ulong p2, TypeInfo* p3, void* p4, uint p5)
    {
        if ((nint)_add_frame_member_ea == 0) throw new System.EntryPointNotFoundException("'add_frame_member_ea' is not available for the loaded IDA SDK version.");
        return _add_frame_member_ea(p0, p1, p2, p3, p4, p5);
    }

    public static byte @add_func_auto_stkpnt(ulong p0, ulong p1, long p2)
    {
        if ((nint)_add_func_auto_stkpnt == 0) throw new System.EntryPointNotFoundException("'add_func_auto_stkpnt' is not available for the loaded IDA SDK version.");
        return _add_func_auto_stkpnt(p0, p1, p2);
    }

    public static byte @add_func_ex(void* p0)
    {
        if ((nint)_add_func_ex == 0) throw new System.EntryPointNotFoundException("'add_func_ex' is not available for the loaded IDA SDK version.");
        return _add_func_ex(p0);
    }

    public static void @add_func_regarg(ulong p0, int p1, TypeInfo* p2, byte* p3)
    {
        if ((nint)_add_func_regarg == 0) throw new System.EntryPointNotFoundException("'add_func_regarg' is not available for the loaded IDA SDK version.");
        _add_func_regarg(p0, p1, p2, p3);
    }

    public static int @add_func_regvar(ulong p0, ulong p1, ulong p2, byte* p3, byte* p4, byte* p5)
    {
        if ((nint)_add_func_regvar == 0) throw new System.EntryPointNotFoundException("'add_func_regvar' is not available for the loaded IDA SDK version.");
        return _add_func_regvar(p0, p1, p2, p3, p4, p5);
    }

    public static byte @add_function_ex(void* p0)
    {
        if ((nint)_add_function_ex == 0) throw new System.EntryPointNotFoundException("'add_function_ex' is not available for the loaded IDA SDK version.");
        return _add_function_ex(p0);
    }

    public static byte @add_hidden_range(ulong p0, ulong p1, byte* p2, byte* p3, byte* p4, uint p5)
    {
        if ((nint)_add_hidden_range == 0) throw new System.EntryPointNotFoundException("'add_hidden_range' is not available for the loaded IDA SDK version.");
        return _add_hidden_range(p0, p1, p2, p3, p4, p5);
    }

    public static void* @add_idc_class(byte* p0, void* p1)
    {
        if ((nint)_add_idc_class == 0) throw new System.EntryPointNotFoundException("'add_idc_class' is not available for the loaded IDA SDK version.");
        return _add_idc_class(p0, p1);
    }

    public static byte @add_idc_func(void* p0)
    {
        if ((nint)_add_idc_func == 0) throw new System.EntryPointNotFoundException("'add_idc_func' is not available for the loaded IDA SDK version.");
        return _add_idc_func(p0);
    }

    public static void* @add_idc_gvar(byte* p0)
    {
        if ((nint)_add_idc_gvar == 0) throw new System.EntryPointNotFoundException("'add_idc_gvar' is not available for the loaded IDA SDK version.");
        return _add_idc_gvar(p0);
    }

    public static byte @add_mapping(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_add_mapping == 0) throw new System.EntryPointNotFoundException("'add_mapping' is not available for the loaded IDA SDK version.");
        return _add_mapping(p0, p1, p2);
    }

    public static void @add_qword(ulong p0, ulong p1)
    {
        if ((nint)_add_qword == 0) throw new System.EntryPointNotFoundException("'add_qword' is not available for the loaded IDA SDK version.");
        _add_qword(p0, p1);
    }

    public static ulong @add_refinfo_dref(void* p0, ulong p1, void* p2, long p3, int p4, int p5)
    {
        if ((nint)_add_refinfo_dref == 0) throw new System.EntryPointNotFoundException("'add_refinfo_dref' is not available for the loaded IDA SDK version.");
        return _add_refinfo_dref(p0, p1, p2, p3, p4, p5);
    }

    public static void @add_regarg(void* p0, int p1, TypeInfo* p2, byte* p3)
    {
        if ((nint)_add_regarg == 0) throw new System.EntryPointNotFoundException("'add_regarg' is not available for the loaded IDA SDK version.");
        _add_regarg(p0, p1, p2, p3);
    }

    public static int @add_regvar(void* p0, ulong p1, ulong p2, byte* p3, byte* p4, byte* p5)
    {
        if ((nint)_add_regvar == 0) throw new System.EntryPointNotFoundException("'add_regvar' is not available for the loaded IDA SDK version.");
        return _add_regvar(p0, p1, p2, p3, p4, p5);
    }

    public static byte @add_segm(ulong p0, ulong p1, ulong p2, byte* p3, byte* p4, int p5)
    {
        if ((nint)_add_segm == 0) throw new System.EntryPointNotFoundException("'add_segm' is not available for the loaded IDA SDK version.");
        return _add_segm(p0, p1, p2, p3, p4, p5);
    }

    public static byte @add_segment_ex(void* p0, int p1)
    {
        if ((nint)_add_segment_ex == 0) throw new System.EntryPointNotFoundException("'add_segment_ex' is not available for the loaded IDA SDK version.");
        return _add_segment_ex(p0, p1);
    }

    public static byte @add_segment_translation(ulong p0, ulong p1)
    {
        if ((nint)_add_segment_translation == 0) throw new System.EntryPointNotFoundException("'add_segment_translation' is not available for the loaded IDA SDK version.");
        return _add_segment_translation(p0, p1);
    }

    public static byte @add_sourcefile(ulong p0, ulong p1, byte* p2)
    {
        if ((nint)_add_sourcefile == 0) throw new System.EntryPointNotFoundException("'add_sourcefile' is not available for the loaded IDA SDK version.");
        return _add_sourcefile(p0, p1, p2);
    }

    public static byte @add_sourcefiles(void* p0)
    {
        if ((nint)_add_sourcefiles == 0) throw new System.EntryPointNotFoundException("'add_sourcefiles' is not available for the loaded IDA SDK version.");
        return _add_sourcefiles(p0);
    }

    public static byte* @add_spaces(byte* p0, nuint p1, nint p2)
    {
        if ((nint)_add_spaces == 0) throw new System.EntryPointNotFoundException("'add_spaces' is not available for the loaded IDA SDK version.");
        return _add_spaces(p0, p1, p2);
    }

    public static byte @add_stkvar(void* p0, void* p1, long p2, int p3)
    {
        if ((nint)_add_stkvar == 0) throw new System.EntryPointNotFoundException("'add_stkvar' is not available for the loaded IDA SDK version.");
        return _add_stkvar(p0, p1, p2, p3);
    }

    public static int @add_til(byte* p0, int p1)
    {
        if ((nint)_add_til == 0) throw new System.EntryPointNotFoundException("'add_til' is not available for the loaded IDA SDK version.");
        return _add_til(p0, p1);
    }

    public static int @add_tryblk(void* p0)
    {
        if ((nint)_add_tryblk == 0) throw new System.EntryPointNotFoundException("'add_tryblk' is not available for the loaded IDA SDK version.");
        return _add_tryblk(p0);
    }

    public static byte @add_user_stkpnt(ulong p0, long p1)
    {
        if ((nint)_add_user_stkpnt == 0) throw new System.EntryPointNotFoundException("'add_user_stkpnt' is not available for the loaded IDA SDK version.");
        return _add_user_stkpnt(p0, p1);
    }

    public static void @add_word(ulong p0, ulong p1)
    {
        if ((nint)_add_word == 0) throw new System.EntryPointNotFoundException("'add_word' is not available for the loaded IDA SDK version.");
        _add_word(p0, p1);
    }

    public static long @adjust_segment_diff(ulong p0, long p1)
    {
        if ((nint)_adjust_segment_diff == 0) throw new System.EntryPointNotFoundException("'adjust_segment_diff' is not available for the loaded IDA SDK version.");
        return _adjust_segment_diff(p0, p1);
    }

    public static ulong @adjust_segment_ea(ulong p0, ulong p1)
    {
        if ((nint)_adjust_segment_ea == 0) throw new System.EntryPointNotFoundException("'adjust_segment_ea' is not available for the loaded IDA SDK version.");
        return _adjust_segment_ea(p0, p1);
    }

    public static ulong @align_down_to_stack(ulong p0)
    {
        if ((nint)_align_down_to_stack == 0) throw new System.EntryPointNotFoundException("'align_down_to_stack' is not available for the loaded IDA SDK version.");
        return _align_down_to_stack(p0);
    }

    public static ulong @align_up_to_stack(ulong p0, ulong p1)
    {
        if ((nint)_align_up_to_stack == 0) throw new System.EntryPointNotFoundException("'align_up_to_stack' is not available for the loaded IDA SDK version.");
        return _align_up_to_stack(p0, p1);
    }

    public static uint @alloc_type_ordinals(void* p0, int p1)
    {
        if ((nint)_alloc_type_ordinals == 0) throw new System.EntryPointNotFoundException("'alloc_type_ordinals' is not available for the loaded IDA SDK version.");
        return _alloc_type_ordinals(p0, p1);
    }

    public static ulong @allocate_selector(ulong p0)
    {
        if ((nint)_allocate_selector == 0) throw new System.EntryPointNotFoundException("'allocate_selector' is not available for the loaded IDA SDK version.");
        return _allocate_selector(p0);
    }

    public static byte @append_abi_opts(byte* p0, byte p1)
    {
        if ((nint)_append_abi_opts == 0) throw new System.EntryPointNotFoundException("'append_abi_opts' is not available for the loaded IDA SDK version.");
        return _append_abi_opts(p0, p1);
    }

    public static byte @append_argloc(void* p0, void* p1)
    {
        if ((nint)_append_argloc == 0) throw new System.EntryPointNotFoundException("'append_argloc' is not available for the loaded IDA SDK version.");
        return _append_argloc(p0, p1);
    }

    public static byte @append_cmt(ulong p0, byte* p1, byte p2)
    {
        if ((nint)_append_cmt == 0) throw new System.EntryPointNotFoundException("'append_cmt' is not available for the loaded IDA SDK version.");
        return _append_cmt(p0, p1, p2);
    }

    public static void @append_disp(QString* p0, long p1, byte p2)
    {
        if ((nint)_append_disp == 0) throw new System.EntryPointNotFoundException("'append_disp' is not available for the loaded IDA SDK version.");
        _append_disp(p0, p1, p2);
    }

    public static byte @append_func_tail(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_append_func_tail == 0) throw new System.EntryPointNotFoundException("'append_func_tail' is not available for the loaded IDA SDK version.");
        return _append_func_tail(p0, p1, p2);
    }

    public static byte @append_func_tail_ea(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_append_func_tail_ea == 0) throw new System.EntryPointNotFoundException("'append_func_tail_ea' is not available for the loaded IDA SDK version.");
        return _append_func_tail_ea(p0, p1, p2);
    }

    public static ulong @append_struct_fields(QString* p0, long* p1, int p2, ulong* p3, int p4, ulong p5, long p6, byte p7)
    {
        if ((nint)_append_struct_fields == 0) throw new System.EntryPointNotFoundException("'append_struct_fields' is not available for the loaded IDA SDK version.");
        return _append_struct_fields(p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static byte @append_tinfo_covered(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_append_tinfo_covered == 0) throw new System.EntryPointNotFoundException("'append_tinfo_covered' is not available for the loaded IDA SDK version.");
        return _append_tinfo_covered(p0, p1, p2);
    }

    public static byte @append_to_flowchart(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_append_to_flowchart == 0) throw new System.EntryPointNotFoundException("'append_to_flowchart' is not available for the loaded IDA SDK version.");
        return _append_to_flowchart(p0, p1, p2);
    }

    public static byte @append_to_func_flow_chart(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_append_to_func_flow_chart == 0) throw new System.EntryPointNotFoundException("'append_to_func_flow_chart' is not available for the loaded IDA SDK version.");
        return _append_to_func_flow_chart(p0, p1, p2);
    }

    public static byte @apply_callee_tinfo(ulong p0, TypeInfo* p1)
    {
        if ((nint)_apply_callee_tinfo == 0) throw new System.EntryPointNotFoundException("'apply_callee_tinfo' is not available for the loaded IDA SDK version.");
        return _apply_callee_tinfo(p0, p1);
    }

    public static byte @apply_cdecl(void* p0, ulong p1, byte* p2, int p3)
    {
        if ((nint)_apply_cdecl == 0) throw new System.EntryPointNotFoundException("'apply_cdecl' is not available for the loaded IDA SDK version.");
        return _apply_cdecl(p0, p1, p2, p3);
    }

    public static byte @apply_fixup(ulong p0, ulong p1, int p2, byte p3)
    {
        if ((nint)_apply_fixup == 0) throw new System.EntryPointNotFoundException("'apply_fixup' is not available for the loaded IDA SDK version.");
        return _apply_fixup(p0, p1, p2, p3);
    }

    public static int @apply_idasgn_to(byte* p0, ulong p1, byte p2)
    {
        if ((nint)_apply_idasgn_to == 0) throw new System.EntryPointNotFoundException("'apply_idasgn_to' is not available for the loaded IDA SDK version.");
        return _apply_idasgn_to(p0, p1, p2);
    }

    public static void @apply_metadata(ulong p0, void* p1, uint p2)
    {
        if ((nint)_apply_metadata == 0) throw new System.EntryPointNotFoundException("'apply_metadata' is not available for the loaded IDA SDK version.");
        _apply_metadata(p0, p1, p2);
    }

    public static byte @apply_named_type(ulong p0, byte* p1)
    {
        if ((nint)_apply_named_type == 0) throw new System.EntryPointNotFoundException("'apply_named_type' is not available for the loaded IDA SDK version.");
        return _apply_named_type(p0, p1);
    }

    public static byte @apply_once_tinfo_and_name(ulong p0, TypeInfo* p1, byte* p2)
    {
        if ((nint)_apply_once_tinfo_and_name == 0) throw new System.EntryPointNotFoundException("'apply_once_tinfo_and_name' is not available for the loaded IDA SDK version.");
        return _apply_once_tinfo_and_name(p0, p1, p2);
    }

    public static byte @apply_startup_sig(ulong p0, byte* p1)
    {
        if ((nint)_apply_startup_sig == 0) throw new System.EntryPointNotFoundException("'apply_startup_sig' is not available for the loaded IDA SDK version.");
        return _apply_startup_sig(p0, p1);
    }

    public static byte @apply_tinfo(ulong p0, TypeInfo* p1, uint p2)
    {
        if ((nint)_apply_tinfo == 0) throw new System.EntryPointNotFoundException("'apply_tinfo' is not available for the loaded IDA SDK version.");
        return _apply_tinfo(p0, p1, p2);
    }

    public static byte @apply_tinfo_to_stkarg(void* p0, void* p1, ulong p2, TypeInfo* p3, byte* p4)
    {
        if ((nint)_apply_tinfo_to_stkarg == 0) throw new System.EntryPointNotFoundException("'apply_tinfo_to_stkarg' is not available for the loaded IDA SDK version.");
        return _apply_tinfo_to_stkarg(p0, p1, p2, p3, p4);
    }

    public static int @asctoreal(byte** p0, void* p1)
    {
        if ((nint)_asctoreal == 0) throw new System.EntryPointNotFoundException("'asctoreal' is not available for the loaded IDA SDK version.");
        return _asctoreal(p0, p1);
    }

    public static byte @atob32(uint* p0, byte* p1)
    {
        if ((nint)_atob32 == 0) throw new System.EntryPointNotFoundException("'atob32' is not available for the loaded IDA SDK version.");
        return _atob32(p0, p1);
    }

    public static byte @atob64(ulong* p0, byte* p1)
    {
        if ((nint)_atob64 == 0) throw new System.EntryPointNotFoundException("'atob64' is not available for the loaded IDA SDK version.");
        return _atob64(p0, p1);
    }

    public static byte @atoea(ulong* p0, byte* p1)
    {
        if ((nint)_atoea == 0) throw new System.EntryPointNotFoundException("'atoea' is not available for the loaded IDA SDK version.");
        return _atoea(p0, p1);
    }

    public static int @atos(ulong* p0, byte* p1)
    {
        if ((nint)_atos == 0) throw new System.EntryPointNotFoundException("'atos' is not available for the loaded IDA SDK version.");
        return _atos(p0, p1);
    }

    public static byte @attach_custom_data_format(int p0, int p1)
    {
        if ((nint)_attach_custom_data_format == 0) throw new System.EntryPointNotFoundException("'attach_custom_data_format' is not available for the loaded IDA SDK version.");
        return _attach_custom_data_format(p0, p1);
    }

    public static void @auto_apply_tail(ulong p0, ulong p1)
    {
        if ((nint)_auto_apply_tail == 0) throw new System.EntryPointNotFoundException("'auto_apply_tail' is not available for the loaded IDA SDK version.");
        _auto_apply_tail(p0, p1);
    }

    public static void @auto_apply_type(ulong p0, ulong p1)
    {
        if ((nint)_auto_apply_type == 0) throw new System.EntryPointNotFoundException("'auto_apply_type' is not available for the loaded IDA SDK version.");
        _auto_apply_type(p0, p1);
    }

    public static void @auto_cancel(ulong p0, ulong p1)
    {
        if ((nint)_auto_cancel == 0) throw new System.EntryPointNotFoundException("'auto_cancel' is not available for the loaded IDA SDK version.");
        _auto_cancel(p0, p1);
    }

    public static ulong @auto_get(int* p0, ulong p1, ulong p2)
    {
        if ((nint)_auto_get == 0) throw new System.EntryPointNotFoundException("'auto_get' is not available for the loaded IDA SDK version.");
        return _auto_get(p0, p1, p2);
    }

    public static byte @auto_is_ok()
    {
        if ((nint)_auto_is_ok == 0) throw new System.EntryPointNotFoundException("'auto_is_ok' is not available for the loaded IDA SDK version.");
        return _auto_is_ok();
    }

    public static byte @auto_make_step(ulong p0, ulong p1)
    {
        if ((nint)_auto_make_step == 0) throw new System.EntryPointNotFoundException("'auto_make_step' is not available for the loaded IDA SDK version.");
        return _auto_make_step(p0, p1);
    }

    public static void @auto_mark_range(ulong p0, ulong p1, int p2)
    {
        if ((nint)_auto_mark_range == 0) throw new System.EntryPointNotFoundException("'auto_mark_range' is not available for the loaded IDA SDK version.");
        _auto_mark_range(p0, p1, p2);
    }

    public static int @auto_recreate_insn(ulong p0)
    {
        if ((nint)_auto_recreate_insn == 0) throw new System.EntryPointNotFoundException("'auto_recreate_insn' is not available for the loaded IDA SDK version.");
        return _auto_recreate_insn(p0);
    }

    public static void @auto_unmark(ulong p0, ulong p1, int p2)
    {
        if ((nint)_auto_unmark == 0) throw new System.EntryPointNotFoundException("'auto_unmark' is not available for the loaded IDA SDK version.");
        _auto_unmark(p0, p1, p2);
    }

    public static byte @auto_wait()
    {
        if ((nint)_auto_wait == 0) throw new System.EntryPointNotFoundException("'auto_wait' is not available for the loaded IDA SDK version.");
        return _auto_wait();
    }

    public static nint @auto_wait_range(ulong p0, ulong p1)
    {
        if ((nint)_auto_wait_range == 0) throw new System.EntryPointNotFoundException("'auto_wait_range' is not available for the loaded IDA SDK version.");
        return _auto_wait_range(p0, p1);
    }

    public static nuint @b2a32(byte* p0, nuint p1, uint p2, int p3, int p4)
    {
        if ((nint)_b2a32 == 0) throw new System.EntryPointNotFoundException("'b2a32' is not available for the loaded IDA SDK version.");
        return _b2a32(p0, p1, p2, p3, p4);
    }

    public static nuint @b2a64(byte* p0, nuint p1, ulong p2, int p3, int p4)
    {
        if ((nint)_b2a64 == 0) throw new System.EntryPointNotFoundException("'b2a64' is not available for the loaded IDA SDK version.");
        return _b2a64(p0, p1, p2, p3, p4);
    }

    public static nuint @b2a_width(int p0, int p1)
    {
        if ((nint)_b2a_width == 0) throw new System.EntryPointNotFoundException("'b2a_width' is not available for the loaded IDA SDK version.");
        return _b2a_width(p0, p1);
    }

    public static byte @back_char(byte** p0)
    {
        if ((nint)_back_char == 0) throw new System.EntryPointNotFoundException("'back_char' is not available for the loaded IDA SDK version.");
        return _back_char(p0);
    }

    public static byte @backup_metadata(ulong p0)
    {
        if ((nint)_backup_metadata == 0) throw new System.EntryPointNotFoundException("'backup_metadata' is not available for the loaded IDA SDK version.");
        return _backup_metadata(p0);
    }

    public static byte @base64_decode(void* p0, byte* p1, nuint p2)
    {
        if ((nint)_base64_decode == 0) throw new System.EntryPointNotFoundException("'base64_decode' is not available for the loaded IDA SDK version.");
        return _base64_decode(p0, p1, p2);
    }

    public static byte @base64_encode(QString* p0, void* p1, nuint p2)
    {
        if ((nint)_base64_encode == 0) throw new System.EntryPointNotFoundException("'base64_encode' is not available for the loaded IDA SDK version.");
        return _base64_encode(p0, p1, p2);
    }

    public static void @begin_type_updating(int p0)
    {
        if ((nint)_begin_type_updating == 0) throw new System.EntryPointNotFoundException("'begin_type_updating' is not available for the loaded IDA SDK version.");
        _begin_type_updating(p0);
    }

    public static ulong @bin_search(ulong p0, ulong p1, void* p2, int p3, nuint* p4)
    {
        if ((nint)_bin_search == 0) throw new System.EntryPointNotFoundException("'bin_search' is not available for the loaded IDA SDK version.");
        return _bin_search(p0, p1, p2, p3, p4);
    }

    public static int @bitcount(ulong p0)
    {
        if ((nint)_bitcount == 0) throw new System.EntryPointNotFoundException("'bitcount' is not available for the loaded IDA SDK version.");
        return _bitcount(p0);
    }

    public static int @bitcountr_zero(ulong p0)
    {
        if ((nint)_bitcountr_zero == 0) throw new System.EntryPointNotFoundException("'bitcountr_zero' is not available for the loaded IDA SDK version.");
        return _bitcountr_zero(p0);
    }

    public static byte @bitrange_t_extract_using_bitrange(void* p0, void* p1, nuint p2, void* p3, nuint p4, byte p5)
    {
        if ((nint)_bitrange_t_extract_using_bitrange == 0) throw new System.EntryPointNotFoundException("'bitrange_t_extract_using_bitrange' is not available for the loaded IDA SDK version.");
        return _bitrange_t_extract_using_bitrange(p0, p1, p2, p3, p4, p5);
    }

    public static byte @bitrange_t_inject_using_bitrange(void* p0, void* p1, nuint p2, void* p3, nuint p4, byte p5)
    {
        if ((nint)_bitrange_t_inject_using_bitrange == 0) throw new System.EntryPointNotFoundException("'bitrange_t_inject_using_bitrange' is not available for the loaded IDA SDK version.");
        return _bitrange_t_inject_using_bitrange(p0, p1, p2, p3, p4, p5);
    }

    public static byte @bookmarks_t_erase(void* p0, uint p1, void* p2)
    {
        if ((nint)_bookmarks_t_erase == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_erase' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_erase(p0, p1, p2);
    }

    public static uint @bookmarks_t_find_index(void* p0, void* p1)
    {
        if ((nint)_bookmarks_t_find_index == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_find_index' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_find_index(p0, p1);
    }

    public static byte @bookmarks_t_get(void* p0, QString* p1, uint* p2, void* p3)
    {
        if ((nint)_bookmarks_t_get == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_get' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_get(p0, p1, p2, p3);
    }

    public static uint @bookmarks_t_get_by_inode(void* p0, QString* p1, ulong p2, void* p3)
    {
        if ((nint)_bookmarks_t_get_by_inode == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_get_by_inode' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_get_by_inode(p0, p1, p2, p3);
    }

    public static byte @bookmarks_t_get_desc(QString* p0, void* p1, uint p2, void* p3)
    {
        if ((nint)_bookmarks_t_get_desc == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_get_desc' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_get_desc(p0, p1, p2, p3);
    }

    public static int @bookmarks_t_get_dirtree_id(void* p0, void* p1)
    {
        if ((nint)_bookmarks_t_get_dirtree_id == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_get_dirtree_id' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_get_dirtree_id(p0, p1);
    }

    public static uint @bookmarks_t_mark(void* p0, uint p1, byte* p2, byte* p3, void* p4)
    {
        if ((nint)_bookmarks_t_mark == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_mark' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_mark(p0, p1, p2, p3, p4);
    }

    public static uint @bookmarks_t_size(void* p0, void* p1)
    {
        if ((nint)_bookmarks_t_size == 0) throw new System.EntryPointNotFoundException("'bookmarks_t_size' is not available for the loaded IDA SDK version.");
        return _bookmarks_t_size(p0, p1);
    }

    public static nuint @btoa32(byte* p0, nuint p1, uint p2, int p3)
    {
        if ((nint)_btoa32 == 0) throw new System.EntryPointNotFoundException("'btoa32' is not available for the loaded IDA SDK version.");
        return _btoa32(p0, p1, p2, p3);
    }

    public static nuint @btoa64(byte* p0, nuint p1, ulong p2, int p3)
    {
        if ((nint)_btoa64 == 0) throw new System.EntryPointNotFoundException("'btoa64' is not available for the loaded IDA SDK version.");
        return _btoa64(p0, p1, p2, p3);
    }

    public static nuint @btoa_width(int p0, ulong p1, int p2)
    {
        if ((nint)_btoa_width == 0) throw new System.EntryPointNotFoundException("'btoa_width' is not available for the loaded IDA SDK version.");
        return _btoa_width(p0, p1, p2);
    }

    public static void @build_anon_type_name(QString* p0, byte* p1, byte* p2)
    {
        if ((nint)_build_anon_type_name == 0) throw new System.EntryPointNotFoundException("'build_anon_type_name' is not available for the loaded IDA SDK version.");
        _build_anon_type_name(p0, p1, p2);
    }

    public static void* @build_loaders_list(void* p0, byte* p1)
    {
        if ((nint)_build_loaders_list == 0) throw new System.EntryPointNotFoundException("'build_loaders_list' is not available for the loaded IDA SDK version.");
        return _build_loaders_list(p0, p1);
    }

    public static void @build_plugin_options(QString* p0, void* p1, byte* p2)
    {
        if ((nint)_build_plugin_options == 0) throw new System.EntryPointNotFoundException("'build_plugin_options' is not available for the loaded IDA SDK version.");
        _build_plugin_options(p0, p1, p2);
    }

    public static byte @build_snapshot_tree(void* p0)
    {
        if ((nint)_build_snapshot_tree == 0) throw new System.EntryPointNotFoundException("'build_snapshot_tree' is not available for the loaded IDA SDK version.");
        return _build_snapshot_tree(p0);
    }

    public static nint @build_stkvar_name(QString* p0, void* p1, long p2)
    {
        if ((nint)_build_stkvar_name == 0) throw new System.EntryPointNotFoundException("'build_stkvar_name' is not available for the loaded IDA SDK version.");
        return _build_stkvar_name(p0, p1, p2);
    }

    public static nint @build_stkvar_name_ea(QString* p0, ulong p1, long p2)
    {
        if ((nint)_build_stkvar_name_ea == 0) throw new System.EntryPointNotFoundException("'build_stkvar_name_ea' is not available for the loaded IDA SDK version.");
        return _build_stkvar_name_ea(p0, p1, p2);
    }

    public static void @build_stkvar_xrefs(void* p0, void* p1, ulong p2, ulong p3)
    {
        if ((nint)_build_stkvar_xrefs == 0) throw new System.EntryPointNotFoundException("'build_stkvar_xrefs' is not available for the loaded IDA SDK version.");
        _build_stkvar_xrefs(p0, p1, p2, p3);
    }

    public static void @build_stkvar_xrefs_ea(void* p0, ulong p1, ulong p2, ulong p3)
    {
        if ((nint)_build_stkvar_xrefs_ea == 0) throw new System.EntryPointNotFoundException("'build_stkvar_xrefs_ea' is not available for the loaded IDA SDK version.");
        _build_stkvar_xrefs_ea(p0, p1, p2, p3);
    }

    public static void @build_strlist()
    {
        if ((nint)_build_strlist == 0) throw new System.EntryPointNotFoundException("'build_strlist' is not available for the loaded IDA SDK version.");
        _build_strlist();
    }

    public static byte @calc_arglocs(void* p0)
    {
        if ((nint)_calc_arglocs == 0) throw new System.EntryPointNotFoundException("'calc_arglocs' is not available for the loaded IDA SDK version.");
        return _calc_arglocs(p0);
    }

    public static ulong @calc_basevalue(ulong p0, ulong p1)
    {
        if ((nint)_calc_basevalue == 0) throw new System.EntryPointNotFoundException("'calc_basevalue' is not available for the loaded IDA SDK version.");
        return _calc_basevalue(p0, p1);
    }

    public static uint @calc_bg_color(ulong p0)
    {
        if ((nint)_calc_bg_color == 0) throw new System.EntryPointNotFoundException("'calc_bg_color' is not available for the loaded IDA SDK version.");
        return _calc_bg_color(p0);
    }

    public static nint @calc_c_cpp_name(QString* p0, byte* p1, TypeInfo* p2, int p3)
    {
        if ((nint)_calc_c_cpp_name == 0) throw new System.EntryPointNotFoundException("'calc_c_cpp_name' is not available for the loaded IDA SDK version.");
        return _calc_c_cpp_name(p0, p1, p2, p3);
    }

    public static uint @calc_crc32(uint p0, void* p1, nuint p2)
    {
        if ((nint)_calc_crc32 == 0) throw new System.EntryPointNotFoundException("'calc_crc32' is not available for the loaded IDA SDK version.");
        return _calc_crc32(p0, p1, p2);
    }

    public static ulong @calc_dataseg(void* p0, int p1, int p2)
    {
        if ((nint)_calc_dataseg == 0) throw new System.EntryPointNotFoundException("'calc_dataseg' is not available for the loaded IDA SDK version.");
        return _calc_dataseg(p0, p1, p2);
    }

    public static int @calc_def_align(ulong p0, int p1, int p2)
    {
        if ((nint)_calc_def_align == 0) throw new System.EntryPointNotFoundException("'calc_def_align' is not available for the loaded IDA SDK version.");
        return _calc_def_align(p0, p1, p2);
    }

    public static uint @calc_file_crc32(void* p0)
    {
        if ((nint)_calc_file_crc32 == 0) throw new System.EntryPointNotFoundException("'calc_file_crc32' is not available for the loaded IDA SDK version.");
        return _calc_file_crc32(p0);
    }

    public static int @calc_fixup_size(ushort p0)
    {
        if ((nint)_calc_fixup_size == 0) throw new System.EntryPointNotFoundException("'calc_fixup_size' is not available for the loaded IDA SDK version.");
        return _calc_fixup_size(p0);
    }

    public static long @calc_frame_offset(void* p0, long p1, void* p2, void* p3)
    {
        if ((nint)_calc_frame_offset == 0) throw new System.EntryPointNotFoundException("'calc_frame_offset' is not available for the loaded IDA SDK version.");
        return _calc_frame_offset(p0, p1, p2, p3);
    }

    public static long @calc_frame_offset_ea(ulong p0, long p1, void* p2, void* p3)
    {
        if ((nint)_calc_frame_offset_ea == 0) throw new System.EntryPointNotFoundException("'calc_frame_offset_ea' is not available for the loaded IDA SDK version.");
        return _calc_frame_offset_ea(p0, p1, p2, p3);
    }

    public static ulong @calc_func_metadata(void* p0, void* p1, void* p2, void* p3)
    {
        if ((nint)_calc_func_metadata == 0) throw new System.EntryPointNotFoundException("'calc_func_metadata' is not available for the loaded IDA SDK version.");
        return _calc_func_metadata(p0, p1, p2, p3);
    }

    public static ulong @calc_func_size(void* p0)
    {
        if ((nint)_calc_func_size == 0) throw new System.EntryPointNotFoundException("'calc_func_size' is not available for the loaded IDA SDK version.");
        return _calc_func_size(p0);
    }

    public static ulong @calc_func_size_ea(ulong p0)
    {
        if ((nint)_calc_func_size_ea == 0) throw new System.EntryPointNotFoundException("'calc_func_size_ea' is not available for the loaded IDA SDK version.");
        return _calc_func_size_ea(p0);
    }

    public static ulong @calc_function_metadata(void* p0, void* p1, ulong p2, void* p3)
    {
        if ((nint)_calc_function_metadata == 0) throw new System.EntryPointNotFoundException("'calc_function_metadata' is not available for the loaded IDA SDK version.");
        return _calc_function_metadata(p0, p1, p2, p3);
    }

    public static int @calc_idasgn_state(int p0)
    {
        if ((nint)_calc_idasgn_state == 0) throw new System.EntryPointNotFoundException("'calc_idasgn_state' is not available for the loaded IDA SDK version.");
        return _calc_idasgn_state(p0);
    }

    public static int @calc_max_align(ulong p0)
    {
        if ((nint)_calc_max_align == 0) throw new System.EntryPointNotFoundException("'calc_max_align' is not available for the loaded IDA SDK version.");
        return _calc_max_align(p0);
    }

    public static ulong @calc_max_item_end(ulong p0, int p1)
    {
        if ((nint)_calc_max_item_end == 0) throw new System.EntryPointNotFoundException("'calc_max_item_end' is not available for the loaded IDA SDK version.");
        return _calc_max_item_end(p0, p1);
    }

    public static int @calc_min_align(ulong p0)
    {
        if ((nint)_calc_min_align == 0) throw new System.EntryPointNotFoundException("'calc_min_align' is not available for the loaded IDA SDK version.");
        return _calc_min_align(p0);
    }

    public static int @calc_number_of_children(void* p0, TypeInfo* p1, byte p2)
    {
        if ((nint)_calc_number_of_children == 0) throw new System.EntryPointNotFoundException("'calc_number_of_children' is not available for the loaded IDA SDK version.");
        return _calc_number_of_children(p0, p1, p2);
    }

    public static ulong @calc_offset_base(ulong p0, int p1)
    {
        if ((nint)_calc_offset_base == 0) throw new System.EntryPointNotFoundException("'calc_offset_base' is not available for the loaded IDA SDK version.");
        return _calc_offset_base(p0, p1);
    }

    public static byte @calc_prefix_color(ulong p0)
    {
        if ((nint)_calc_prefix_color == 0) throw new System.EntryPointNotFoundException("'calc_prefix_color' is not available for the loaded IDA SDK version.");
        return _calc_prefix_color(p0);
    }

    public static ulong @calc_probable_base_by_value(ulong p0, ulong p1)
    {
        if ((nint)_calc_probable_base_by_value == 0) throw new System.EntryPointNotFoundException("'calc_probable_base_by_value' is not available for the loaded IDA SDK version.");
        return _calc_probable_base_by_value(p0, p1);
    }

    public static byte @calc_reference_data(ulong* p0, ulong* p1, ulong p2, void* p3, long p4)
    {
        if ((nint)_calc_reference_data == 0) throw new System.EntryPointNotFoundException("'calc_reference_data' is not available for the loaded IDA SDK version.");
        return _calc_reference_data(p0, p1, p2, p3, p4);
    }

    public static byte @calc_retloc(void* p0)
    {
        if ((nint)_calc_retloc == 0) throw new System.EntryPointNotFoundException("'calc_retloc' is not available for the loaded IDA SDK version.");
        return _calc_retloc(p0);
    }

    public static ulong @calc_stkvar_struc_offset(void* p0, void* p1, int p2)
    {
        if ((nint)_calc_stkvar_struc_offset == 0) throw new System.EntryPointNotFoundException("'calc_stkvar_struc_offset' is not available for the loaded IDA SDK version.");
        return _calc_stkvar_struc_offset(p0, p1, p2);
    }

    public static ulong @calc_stkvar_struc_offset_ea(ulong p0, void* p1, int p2)
    {
        if ((nint)_calc_stkvar_struc_offset_ea == 0) throw new System.EntryPointNotFoundException("'calc_stkvar_struc_offset_ea' is not available for the loaded IDA SDK version.");
        return _calc_stkvar_struc_offset_ea(p0, p1, p2);
    }

    public static byte @calc_switch_cases(void* p0, void* p1, ulong p2, void* p3)
    {
        if ((nint)_calc_switch_cases == 0) throw new System.EntryPointNotFoundException("'calc_switch_cases' is not available for the loaded IDA SDK version.");
        return _calc_switch_cases(p0, p1, p2, p3);
    }

    public static ulong @calc_thunk_func_target(void* p0, ulong* p1)
    {
        if ((nint)_calc_thunk_func_target == 0) throw new System.EntryPointNotFoundException("'calc_thunk_func_target' is not available for the loaded IDA SDK version.");
        return _calc_thunk_func_target(p0, p1);
    }

    public static ulong @calc_thunk_function_target(void* p0, ulong* p1)
    {
        if ((nint)_calc_thunk_function_target == 0) throw new System.EntryPointNotFoundException("'calc_thunk_function_target' is not available for the loaded IDA SDK version.");
        return _calc_thunk_function_target(p0, p1);
    }

    public static byte @calc_tinfo_gaps(void* p0, ulong p1)
    {
        if ((nint)_calc_tinfo_gaps == 0) throw new System.EntryPointNotFoundException("'calc_tinfo_gaps' is not available for the loaded IDA SDK version.");
        return _calc_tinfo_gaps(p0, p1);
    }

    public static byte @calc_varglocs(void* p0, void* p1, void* p2, int p3)
    {
        if ((nint)_calc_varglocs == 0) throw new System.EntryPointNotFoundException("'calc_varglocs' is not available for the loaded IDA SDK version.");
        return _calc_varglocs(p0, p1, p2, p3);
    }

    public static int @call_system(byte* p0)
    {
        if ((nint)_call_system == 0) throw new System.EntryPointNotFoundException("'call_system' is not available for the loaded IDA SDK version.");
        return _call_system(p0);
    }

    public static ulong @can_be_off32(ulong p0)
    {
        if ((nint)_can_be_off32 == 0) throw new System.EntryPointNotFoundException("'can_be_off32' is not available for the loaded IDA SDK version.");
        return _can_be_off32(p0);
    }

    public static byte @can_define_item(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_can_define_item == 0) throw new System.EntryPointNotFoundException("'can_define_item' is not available for the loaded IDA SDK version.");
        return _can_define_item(p0, p1, p2);
    }

    public static byte* @cfg_get_cc_parm(byte p0, byte* p1)
    {
        if ((nint)_cfg_get_cc_parm == 0) throw new System.EntryPointNotFoundException("'cfg_get_cc_parm' is not available for the loaded IDA SDK version.");
        return _cfg_get_cc_parm(p0, p1);
    }

    public static byte* @cfgopt_t__apply(void* p0, int p1, void* p2)
    {
        if ((nint)_cfgopt_t__apply == 0) throw new System.EntryPointNotFoundException("'cfgopt_t__apply' is not available for the loaded IDA SDK version.");
        return _cfgopt_t__apply(p0, p1, p2);
    }

    public static byte* @cfgopt_t__apply2(void* p0, int p1, void* p2, void* p3)
    {
        if ((nint)_cfgopt_t__apply2 == 0) throw new System.EntryPointNotFoundException("'cfgopt_t__apply2' is not available for the loaded IDA SDK version.");
        return _cfgopt_t__apply2(p0, p1, p2, p3);
    }

    public static byte* @cfgopt_t__apply3(void* p0, void* p1, int p2, void* p3, void* p4)
    {
        if ((nint)_cfgopt_t__apply3 == 0) throw new System.EntryPointNotFoundException("'cfgopt_t__apply3' is not available for the loaded IDA SDK version.");
        return _cfgopt_t__apply3(p0, p1, p2, p3, p4);
    }

    public static byte @change_codepage(QString* p0, byte* p1, int p2, int p3)
    {
        if ((nint)_change_codepage == 0) throw new System.EntryPointNotFoundException("'change_codepage' is not available for the loaded IDA SDK version.");
        return _change_codepage(p0, p1, p2, p3);
    }

    public static int @change_segment_status(void* p0, byte p1)
    {
        if ((nint)_change_segment_status == 0) throw new System.EntryPointNotFoundException("'change_segment_status' is not available for the loaded IDA SDK version.");
        return _change_segment_status(p0, p1);
    }

    public static int @change_segment_status_by_ea(ulong p0, byte p1)
    {
        if ((nint)_change_segment_status_by_ea == 0) throw new System.EntryPointNotFoundException("'change_segment_status_by_ea' is not available for the loaded IDA SDK version.");
        return _change_segment_status_by_ea(p0, p1);
    }

    public static int @change_storage_type(ulong p0, ulong p1, int p2)
    {
        if ((nint)_change_storage_type == 0) throw new System.EntryPointNotFoundException("'change_storage_type' is not available for the loaded IDA SDK version.");
        return _change_storage_type(p0, p1, p2);
    }

    public static int @check_flat_jump_table(void* p0, ulong p1, int p2)
    {
        if ((nint)_check_flat_jump_table == 0) throw new System.EntryPointNotFoundException("'check_flat_jump_table' is not available for the loaded IDA SDK version.");
        return _check_flat_jump_table(p0, p1, p2);
    }

    public static void @check_spoiled_jpt(void* p0, void* p1)
    {
        if ((nint)_check_spoiled_jpt == 0) throw new System.EntryPointNotFoundException("'check_spoiled_jpt' is not available for the loaded IDA SDK version.");
        _check_spoiled_jpt(p0, p1);
    }

    public static byte @choose_ioport_device2(QString* p0, byte* p1, void* p2)
    {
        if ((nint)_choose_ioport_device2 == 0) throw new System.EntryPointNotFoundException("'choose_ioport_device2' is not available for the loaded IDA SDK version.");
        return _choose_ioport_device2(p0, p1, p2);
    }

    public static uint @choose_local_tinfo(void* p0, byte* p1, void* p2, uint p3, void* p4)
    {
        if ((nint)_choose_local_tinfo == 0) throw new System.EntryPointNotFoundException("'choose_local_tinfo' is not available for the loaded IDA SDK version.");
        return _choose_local_tinfo(p0, p1, p2, p3, p4);
    }

    public static uint @choose_local_tinfo_and_delta(int* p0, void* p1, byte* p2, void* p3, uint p4, void* p5)
    {
        if ((nint)_choose_local_tinfo_and_delta == 0) throw new System.EntryPointNotFoundException("'choose_local_tinfo_and_delta' is not available for the loaded IDA SDK version.");
        return _choose_local_tinfo_and_delta(p0, p1, p2, p3, p4, p5);
    }

    public static byte @choose_named_type(void* p0, void* p1, byte* p2, int p3, void* p4)
    {
        if ((nint)_choose_named_type == 0) throw new System.EntryPointNotFoundException("'choose_named_type' is not available for the loaded IDA SDK version.");
        return _choose_named_type(p0, p1, p2, p3, p4);
    }

    public static ulong @chunk_size(ulong p0)
    {
        if ((nint)_chunk_size == 0) throw new System.EntryPointNotFoundException("'chunk_size' is not available for the loaded IDA SDK version.");
        return _chunk_size(p0);
    }

    public static ulong @chunk_start(ulong p0)
    {
        if ((nint)_chunk_start == 0) throw new System.EntryPointNotFoundException("'chunk_start' is not available for the loaded IDA SDK version.");
        return _chunk_start(p0);
    }

    public static int @cleanup_appcall(int p0)
    {
        if ((nint)_cleanup_appcall == 0) throw new System.EntryPointNotFoundException("'cleanup_appcall' is not available for the loaded IDA SDK version.");
        return _cleanup_appcall(p0);
    }

    public static void @cleanup_argloc(void* p0)
    {
        if ((nint)_cleanup_argloc == 0) throw new System.EntryPointNotFoundException("'cleanup_argloc' is not available for the loaded IDA SDK version.");
        _cleanup_argloc(p0);
    }

    public static byte @cleanup_name(QString* p0, ulong p1, byte* p2, uint p3)
    {
        if ((nint)_cleanup_name == 0) throw new System.EntryPointNotFoundException("'cleanup_name' is not available for the loaded IDA SDK version.");
        return _cleanup_name(p0, p1, p2, p3);
    }

    public static void @clear_strlist()
    {
        if ((nint)_clear_strlist == 0) throw new System.EntryPointNotFoundException("'clear_strlist' is not available for the loaded IDA SDK version.");
        _clear_strlist();
    }

    public static void @clear_tinfo_t(TypeInfo* p0)
    {
        if ((nint)_clear_tinfo_t == 0) throw new System.EntryPointNotFoundException("'clear_tinfo_t' is not available for the loaded IDA SDK version.");
        _clear_tinfo_t(p0);
    }

    public static void @cliopts_t_add(void* p0, void* p1, nuint p2)
    {
        if ((nint)_cliopts_t_add == 0) throw new System.EntryPointNotFoundException("'cliopts_t_add' is not available for the loaded IDA SDK version.");
        _cliopts_t_add(p0, p1, p2);
    }

    public static void* @cliopts_t_find_long(void* p0, byte* p1)
    {
        if ((nint)_cliopts_t_find_long == 0) throw new System.EntryPointNotFoundException("'cliopts_t_find_long' is not available for the loaded IDA SDK version.");
        return _cliopts_t_find_long(p0, p1);
    }

    public static void* @cliopts_t_find_short(void* p0, byte p1)
    {
        if ((nint)_cliopts_t_find_short == 0) throw new System.EntryPointNotFoundException("'cliopts_t_find_short' is not available for the loaded IDA SDK version.");
        return _cliopts_t_find_short(p0, p1);
    }

    public static void @cliopts_t_usage(void* p0, byte p1)
    {
        if ((nint)_cliopts_t_usage == 0) throw new System.EntryPointNotFoundException("'cliopts_t_usage' is not available for the loaded IDA SDK version.");
        _cliopts_t_usage(p0, p1);
    }

    public static void @close_database(byte p0)
    {
        if ((nint)_close_database == 0) throw new System.EntryPointNotFoundException("'close_database' is not available for the loaded IDA SDK version.");
        _close_database(p0);
    }

    public static void @close_linput(void* p0)
    {
        if ((nint)_close_linput == 0) throw new System.EntryPointNotFoundException("'close_linput' is not available for the loaded IDA SDK version.");
        _close_linput(p0);
    }

    public static byte* @closing_comment()
    {
        if ((nint)_closing_comment == 0) throw new System.EntryPointNotFoundException("'closing_comment' is not available for the loaded IDA SDK version.");
        return _closing_comment();
    }

    public static void @clr_abits(ulong p0, uint p1)
    {
        if ((nint)_clr_abits == 0) throw new System.EntryPointNotFoundException("'clr_abits' is not available for the loaded IDA SDK version.");
        _clr_abits(p0, p1);
    }

    public static byte @clr_lzero(ulong p0, int p1)
    {
        if ((nint)_clr_lzero == 0) throw new System.EntryPointNotFoundException("'clr_lzero' is not available for the loaded IDA SDK version.");
        return _clr_lzero(p0, p1);
    }

    public static void* @clr_module_data(int p0)
    {
        if ((nint)_clr_module_data == 0) throw new System.EntryPointNotFoundException("'clr_module_data' is not available for the loaded IDA SDK version.");
        return _clr_module_data(p0);
    }

    public static void @clr_node_info(ulong p0, int p1, uint p2)
    {
        if ((nint)_clr_node_info == 0) throw new System.EntryPointNotFoundException("'clr_node_info' is not available for the loaded IDA SDK version.");
        _clr_node_info(p0, p1, p2);
    }

    public static byte @clr_op_type(ulong p0, int p1)
    {
        if ((nint)_clr_op_type == 0) throw new System.EntryPointNotFoundException("'clr_op_type' is not available for the loaded IDA SDK version.");
        return _clr_op_type(p0, p1);
    }

    public static void @code_highlight_block(void* p0, void* p1, QString* p2)
    {
        if ((nint)_code_highlight_block == 0) throw new System.EntryPointNotFoundException("'code_highlight_block' is not available for the loaded IDA SDK version.");
        _code_highlight_block(p0, p1, p2);
    }

    public static void @combine_regs_jpt(void* p0, void* p1, void* p2, ulong p3)
    {
        if ((nint)_combine_regs_jpt == 0) throw new System.EntryPointNotFoundException("'combine_regs_jpt' is not available for the loaded IDA SDK version.");
        _combine_regs_jpt(p0, p1, p2, p3);
    }

    public static int @compact_numbered_types(void* p0, uint p1, void* p2, int p3)
    {
        if ((nint)_compact_numbered_types == 0) throw new System.EntryPointNotFoundException("'compact_numbered_types' is not available for the loaded IDA SDK version.");
        return _compact_numbered_types(p0, p1, p2, p3);
    }

    public static byte @compact_til(void* p0)
    {
        if ((nint)_compact_til == 0) throw new System.EntryPointNotFoundException("'compact_til' is not available for the loaded IDA SDK version.");
        return _compact_til(p0);
    }

    public static int @compare_arglocs(void* p0, void* p1)
    {
        if ((nint)_compare_arglocs == 0) throw new System.EntryPointNotFoundException("'compare_arglocs' is not available for the loaded IDA SDK version.");
        return _compare_arglocs(p0, p1);
    }

    public static int @compare_bpt_locs(void* p0, void* p1)
    {
        if ((nint)_compare_bpt_locs == 0) throw new System.EntryPointNotFoundException("'compare_bpt_locs' is not available for the loaded IDA SDK version.");
        return _compare_bpt_locs(p0, p1);
    }

    public static byte @compare_tinfo(ulong p0, ulong p1, int p2)
    {
        if ((nint)_compare_tinfo == 0) throw new System.EntryPointNotFoundException("'compare_tinfo' is not available for the loaded IDA SDK version.");
        return _compare_tinfo(p0, p1, p2);
    }

    public static byte @compile_idc_file(byte* p0, QString* p1, int p2)
    {
        if ((nint)_compile_idc_file == 0) throw new System.EntryPointNotFoundException("'compile_idc_file' is not available for the loaded IDA SDK version.");
        return _compile_idc_file(p0, p1, p2);
    }

    public static byte @compile_idc_snippet(byte* p0, byte* p1, QString* p2, void* p3, byte p4)
    {
        if ((nint)_compile_idc_snippet == 0) throw new System.EntryPointNotFoundException("'compile_idc_snippet' is not available for the loaded IDA SDK version.");
        return _compile_idc_snippet(p0, p1, p2, p3, p4);
    }

    public static byte @compile_idc_text(byte* p0, QString* p1, void* p2, byte p3)
    {
        if ((nint)_compile_idc_text == 0) throw new System.EntryPointNotFoundException("'compile_idc_text' is not available for the loaded IDA SDK version.");
        return _compile_idc_text(p0, p1, p2, p3);
    }

    public static byte @construct_macro(void* p0, void* p1, byte p2)
    {
        if ((nint)_construct_macro == 0) throw new System.EntryPointNotFoundException("'construct_macro' is not available for the loaded IDA SDK version.");
        return _construct_macro(p0, p1, p2);
    }

    public static void @copy_argloc(void* p0, void* p1)
    {
        if ((nint)_copy_argloc == 0) throw new System.EntryPointNotFoundException("'copy_argloc' is not available for the loaded IDA SDK version.");
        _copy_argloc(p0, p1);
    }

    public static void @copy_debug_event(void* p0, void* p1)
    {
        if ((nint)_copy_debug_event == 0) throw new System.EntryPointNotFoundException("'copy_debug_event' is not available for the loaded IDA SDK version.");
        _copy_debug_event(p0, p1);
    }

    public static int @copy_idcv(void* p0, void* p1)
    {
        if ((nint)_copy_idcv == 0) throw new System.EntryPointNotFoundException("'copy_idcv' is not available for the loaded IDA SDK version.");
        return _copy_idcv(p0, p1);
    }

    public static uint @copy_named_type(void* p0, void* p1, byte* p2)
    {
        if ((nint)_copy_named_type == 0) throw new System.EntryPointNotFoundException("'copy_named_type' is not available for the loaded IDA SDK version.");
        return _copy_named_type(p0, p1, p2);
    }

    public static void @copy_sreg_ranges(int p0, int p1, byte p2)
    {
        if ((nint)_copy_sreg_ranges == 0) throw new System.EntryPointNotFoundException("'copy_sreg_ranges' is not available for the loaded IDA SDK version.");
        _copy_sreg_ranges(p0, p1, p2);
    }

    public static void @copy_tinfo_t(TypeInfo* p0, TypeInfo* p1)
    {
        if ((nint)_copy_tinfo_t == 0) throw new System.EntryPointNotFoundException("'copy_tinfo_t' is not available for the loaded IDA SDK version.");
        _copy_tinfo_t(p0, p1);
    }

    public static ulong @correct_address(ulong p0, ulong p1, ulong p2, ulong p3, byte p4)
    {
        if ((nint)_correct_address == 0) throw new System.EntryPointNotFoundException("'correct_address' is not available for the loaded IDA SDK version.");
        return _correct_address(p0, p1, p2, p3, p4);
    }

    public static int @cpu2ieee(void* p0, void* p1, int p2)
    {
        if ((nint)_cpu2ieee == 0) throw new System.EntryPointNotFoundException("'cpu2ieee' is not available for the loaded IDA SDK version.");
        return _cpu2ieee(p0, p1, p2);
    }

    public static byte @create_16bit_data(ulong p0, ulong p1)
    {
        if ((nint)_create_16bit_data == 0) throw new System.EntryPointNotFoundException("'create_16bit_data' is not available for the loaded IDA SDK version.");
        return _create_16bit_data(p0, p1);
    }

    public static byte @create_32bit_data(ulong p0, ulong p1)
    {
        if ((nint)_create_32bit_data == 0) throw new System.EntryPointNotFoundException("'create_32bit_data' is not available for the loaded IDA SDK version.");
        return _create_32bit_data(p0, p1);
    }

    public static byte @create_align(ulong p0, ulong p1, int p2)
    {
        if ((nint)_create_align == 0) throw new System.EntryPointNotFoundException("'create_align' is not available for the loaded IDA SDK version.");
        return _create_align(p0, p1, p2);
    }

    public static void* @create_bytearray_linput(byte* p0, nuint p1)
    {
        if ((nint)_create_bytearray_linput == 0) throw new System.EntryPointNotFoundException("'create_bytearray_linput' is not available for the loaded IDA SDK version.");
        return _create_bytearray_linput(p0, p1);
    }

    public static byte @create_data(ulong p0, ulong p1, ulong p2, ulong p3)
    {
        if ((nint)_create_data == 0) throw new System.EntryPointNotFoundException("'create_data' is not available for the loaded IDA SDK version.");
        return _create_data(p0, p1, p2, p3);
    }

    public static void* @create_dirtree(void* p0, void* p1)
    {
        if ((nint)_create_dirtree == 0) throw new System.EntryPointNotFoundException("'create_dirtree' is not available for the loaded IDA SDK version.");
        return _create_dirtree(p0, p1);
    }

    public static void @create_filename_cmt()
    {
        if ((nint)_create_filename_cmt == 0) throw new System.EntryPointNotFoundException("'create_filename_cmt' is not available for the loaded IDA SDK version.");
        _create_filename_cmt();
    }

    public static void @create_func_flow_chart(void* p0)
    {
        if ((nint)_create_func_flow_chart == 0) throw new System.EntryPointNotFoundException("'create_func_flow_chart' is not available for the loaded IDA SDK version.");
        _create_func_flow_chart(p0);
    }

    public static void* @create_generic_linput(void* p0)
    {
        if ((nint)_create_generic_linput == 0) throw new System.EntryPointNotFoundException("'create_generic_linput' is not available for the loaded IDA SDK version.");
        return _create_generic_linput(p0);
    }

    public static byte @create_idcv_ref(void* p0, void* p1)
    {
        if ((nint)_create_idcv_ref == 0) throw new System.EntryPointNotFoundException("'create_idcv_ref' is not available for the loaded IDA SDK version.");
        return _create_idcv_ref(p0, p1);
    }

    public static int @create_insn(ulong p0, void* p1)
    {
        if ((nint)_create_insn == 0) throw new System.EntryPointNotFoundException("'create_insn' is not available for the loaded IDA SDK version.");
        return _create_insn(p0, p1);
    }

    public static void* @create_lexer(byte** p0, nuint p1, void* p2, uint p3)
    {
        if ((nint)_create_lexer == 0) throw new System.EntryPointNotFoundException("'create_lexer' is not available for the loaded IDA SDK version.");
        return _create_lexer(p0, p1, p2, p3);
    }

    public static void* @create_memory_linput(ulong p0, ulong p1)
    {
        if ((nint)_create_memory_linput == 0) throw new System.EntryPointNotFoundException("'create_memory_linput' is not available for the loaded IDA SDK version.");
        return _create_memory_linput(p0, p1);
    }

    public static byte @create_multirange_func_flow_chart(void* p0, void* p1)
    {
        if ((nint)_create_multirange_func_flow_chart == 0) throw new System.EntryPointNotFoundException("'create_multirange_func_flow_chart' is not available for the loaded IDA SDK version.");
        return _create_multirange_func_flow_chart(p0, p1);
    }

    public static byte @create_multirange_qflow_chart(void* p0, void* p1)
    {
        if ((nint)_create_multirange_qflow_chart == 0) throw new System.EntryPointNotFoundException("'create_multirange_qflow_chart' is not available for the loaded IDA SDK version.");
        return _create_multirange_qflow_chart(p0, p1);
    }

    public static void* @create_nodeval_merge_handler2(void* p0, byte* p1, int p2, byte* p3, byte p4, uint p5, void* p6, byte p7)
    {
        if ((nint)_create_nodeval_merge_handler2 == 0) throw new System.EntryPointNotFoundException("'create_nodeval_merge_handler2' is not available for the loaded IDA SDK version.");
        return _create_nodeval_merge_handler2(p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void @create_nodeval_merge_handlers(void* p0, void* p1, int p2, byte* p3, void* p4, nuint p5, byte p6)
    {
        if ((nint)_create_nodeval_merge_handlers == 0) throw new System.EntryPointNotFoundException("'create_nodeval_merge_handlers' is not available for the loaded IDA SDK version.");
        _create_nodeval_merge_handlers(p0, p1, p2, p3, p4, p5, p6);
    }

    public static void @create_nodeval_merge_handlers2(void* p0, void* p1, int p2, byte* p3, void* p4, nuint p5, byte p6)
    {
        if ((nint)_create_nodeval_merge_handlers2 == 0) throw new System.EntryPointNotFoundException("'create_nodeval_merge_handlers2' is not available for the loaded IDA SDK version.");
        _create_nodeval_merge_handlers2(p0, p1, p2, p3, p4, p5, p6);
    }

    public static nint @create_numbered_type_name(QString* p0, int p1)
    {
        if ((nint)_create_numbered_type_name == 0) throw new System.EntryPointNotFoundException("'create_numbered_type_name' is not available for the loaded IDA SDK version.");
        return _create_numbered_type_name(p0, p1);
    }

    public static void* @create_outctx(ulong p0, ulong p1, int p2)
    {
        if ((nint)_create_outctx == 0) throw new System.EntryPointNotFoundException("'create_outctx' is not available for the loaded IDA SDK version.");
        return _create_outctx(p0, p1, p2);
    }

    public static void @create_qflow_chart(void* p0)
    {
        if ((nint)_create_qflow_chart == 0) throw new System.EntryPointNotFoundException("'create_qflow_chart' is not available for the loaded IDA SDK version.");
        _create_qflow_chart(p0);
    }

    public static void @create_std_modmerge_handlers(void* p0, int p1, void* p2, void* p3, nuint p4)
    {
        if ((nint)_create_std_modmerge_handlers == 0) throw new System.EntryPointNotFoundException("'create_std_modmerge_handlers' is not available for the loaded IDA SDK version.");
        _create_std_modmerge_handlers(p0, p1, p2, p3, p4);
    }

    public static void @create_std_modmerge_handlers2(void* p0, int p1, void* p2, void* p3, nuint p4)
    {
        if ((nint)_create_std_modmerge_handlers2 == 0) throw new System.EntryPointNotFoundException("'create_std_modmerge_handlers2' is not available for the loaded IDA SDK version.");
        _create_std_modmerge_handlers2(p0, p1, p2, p3, p4);
    }

    public static byte @create_strlit(ulong p0, nuint p1, int p2)
    {
        if ((nint)_create_strlit == 0) throw new System.EntryPointNotFoundException("'create_strlit' is not available for the loaded IDA SDK version.");
        return _create_strlit(p0, p1, p2);
    }

    public static byte @create_switch_table(ulong p0, void* p1)
    {
        if ((nint)_create_switch_table == 0) throw new System.EntryPointNotFoundException("'create_switch_table' is not available for the loaded IDA SDK version.");
        return _create_switch_table(p0, p1);
    }

    public static void @create_switch_xrefs(ulong p0, void* p1)
    {
        if ((nint)_create_switch_xrefs == 0) throw new System.EntryPointNotFoundException("'create_switch_xrefs' is not available for the loaded IDA SDK version.");
        _create_switch_xrefs(p0, p1);
    }

    public static byte @create_tinfo(TypeInfo* p0, byte p1, byte p2, void* p3)
    {
        if ((nint)_create_tinfo == 0) throw new System.EntryPointNotFoundException("'create_tinfo' is not available for the loaded IDA SDK version.");
        return _create_tinfo(p0, p1, p2, p3);
    }

    public static byte @create_undo_point(byte* p0, nuint p1)
    {
        if ((nint)_create_undo_point == 0) throw new System.EntryPointNotFoundException("'create_undo_point' is not available for the loaded IDA SDK version.");
        return _create_undo_point(p0, p1);
    }

    public static byte @create_xrefs_from(ulong p0)
    {
        if ((nint)_create_xrefs_from == 0) throw new System.EntryPointNotFoundException("'create_xrefs_from' is not available for the loaded IDA SDK version.");
        return _create_xrefs_from(p0);
    }

    public static void* @create_zip_linput(void* p0, nint p1, int p2)
    {
        if ((nint)_create_zip_linput == 0) throw new System.EntryPointNotFoundException("'create_zip_linput' is not available for the loaded IDA SDK version.");
        return _create_zip_linput(p0, p1, p2);
    }

    public static int @dbg_appcall(void* p0, ulong p1, int p2, TypeInfo* p3, void* p4, nuint p5)
    {
        if ((nint)_dbg_appcall == 0) throw new System.EntryPointNotFoundException("'dbg_appcall' is not available for the loaded IDA SDK version.");
        return _dbg_appcall(p0, p1, p2, p3, p4, p5);
    }

    public static nint @dbg_get_input_path(byte* p0, nuint p1)
    {
        if ((nint)_dbg_get_input_path == 0) throw new System.EntryPointNotFoundException("'dbg_get_input_path' is not available for the loaded IDA SDK version.");
        return _dbg_get_input_path(p0, p1);
    }

    public static int @decode_insn(void* p0, ulong p1)
    {
        if ((nint)_decode_insn == 0) throw new System.EntryPointNotFoundException("'decode_insn' is not available for the loaded IDA SDK version.");
        return _decode_insn(p0, p1);
    }

    public static ulong @decode_preceding_insn(void* p0, ulong p1, byte* p2)
    {
        if ((nint)_decode_preceding_insn == 0) throw new System.EntryPointNotFoundException("'decode_preceding_insn' is not available for the loaded IDA SDK version.");
        return _decode_preceding_insn(p0, p1, p2);
    }

    public static ulong @decode_prev_insn(void* p0, ulong p1)
    {
        if ((nint)_decode_prev_insn == 0) throw new System.EntryPointNotFoundException("'decode_prev_insn' is not available for the loaded IDA SDK version.");
        return _decode_prev_insn(p0, p1);
    }

    public static byte @decorate_name(QString* p0, byte* p1, byte p2, uint p3, TypeInfo* p4)
    {
        if ((nint)_decorate_name == 0) throw new System.EntryPointNotFoundException("'decorate_name' is not available for the loaded IDA SDK version.");
        return _decorate_name(p0, p1, p2, p3, p4);
    }

    public static int @deep_copy_idcv(void* p0, void* p1)
    {
        if ((nint)_deep_copy_idcv == 0) throw new System.EntryPointNotFoundException("'deep_copy_idcv' is not available for the loaded IDA SDK version.");
        return _deep_copy_idcv(p0, p1);
    }

    public static byte @define_stkvar(void* p0, byte* p1, long p2, TypeInfo* p3, void* p4)
    {
        if ((nint)_define_stkvar == 0) throw new System.EntryPointNotFoundException("'define_stkvar' is not available for the loaded IDA SDK version.");
        return _define_stkvar(p0, p1, p2, p3, p4);
    }

    public static byte @define_stkvar_ea(ulong p0, byte* p1, long p2, TypeInfo* p3, void* p4)
    {
        if ((nint)_define_stkvar_ea == 0) throw new System.EntryPointNotFoundException("'define_stkvar_ea' is not available for the loaded IDA SDK version.");
        return _define_stkvar_ea(p0, p1, p2, p3, p4);
    }

    public static void @del_aflags(ulong p0)
    {
        if ((nint)_del_aflags == 0) throw new System.EntryPointNotFoundException("'del_aflags' is not available for the loaded IDA SDK version.");
        _del_aflags(p0);
    }

    public static byte @del_cref(ulong p0, ulong p1, byte p2)
    {
        if ((nint)_del_cref == 0) throw new System.EntryPointNotFoundException("'del_cref' is not available for the loaded IDA SDK version.");
        return _del_cref(p0, p1, p2);
    }

    public static void @del_debug_names(ulong p0, ulong p1)
    {
        if ((nint)_del_debug_names == 0) throw new System.EntryPointNotFoundException("'del_debug_names' is not available for the loaded IDA SDK version.");
        _del_debug_names(p0, p1);
    }

    public static void @del_dref(ulong p0, ulong p1)
    {
        if ((nint)_del_dref == 0) throw new System.EntryPointNotFoundException("'del_dref' is not available for the loaded IDA SDK version.");
        _del_dref(p0, p1);
    }

    public static byte @del_encoding(int p0)
    {
        if ((nint)_del_encoding == 0) throw new System.EntryPointNotFoundException("'del_encoding' is not available for the loaded IDA SDK version.");
        return _del_encoding(p0);
    }

    public static byte @del_extra_cmt(ulong p0, int p1)
    {
        if ((nint)_del_extra_cmt == 0) throw new System.EntryPointNotFoundException("'del_extra_cmt' is not available for the loaded IDA SDK version.");
        return _del_extra_cmt(p0, p1);
    }

    public static void @del_fixup(ulong p0)
    {
        if ((nint)_del_fixup == 0) throw new System.EntryPointNotFoundException("'del_fixup' is not available for the loaded IDA SDK version.");
        _del_fixup(p0);
    }

    public static byte @del_frame(void* p0)
    {
        if ((nint)_del_frame == 0) throw new System.EntryPointNotFoundException("'del_frame' is not available for the loaded IDA SDK version.");
        return _del_frame(p0);
    }

    public static byte @del_frame_ea(ulong p0)
    {
        if ((nint)_del_frame_ea == 0) throw new System.EntryPointNotFoundException("'del_frame_ea' is not available for the loaded IDA SDK version.");
        return _del_frame_ea(p0);
    }

    public static byte @del_func(ulong p0)
    {
        if ((nint)_del_func == 0) throw new System.EntryPointNotFoundException("'del_func' is not available for the loaded IDA SDK version.");
        return _del_func(p0);
    }

    public static int @del_func_regvar(ulong p0, ulong p1, ulong p2, byte* p3)
    {
        if ((nint)_del_func_regvar == 0) throw new System.EntryPointNotFoundException("'del_func_regvar' is not available for the loaded IDA SDK version.");
        return _del_func_regvar(p0, p1, p2, p3);
    }

    public static byte @del_func_stkpnt(ulong p0, ulong p1)
    {
        if ((nint)_del_func_stkpnt == 0) throw new System.EntryPointNotFoundException("'del_func_stkpnt' is not available for the loaded IDA SDK version.");
        return _del_func_stkpnt(p0, p1);
    }

    public static byte @del_hidden_range(ulong p0)
    {
        if ((nint)_del_hidden_range == 0) throw new System.EntryPointNotFoundException("'del_hidden_range' is not available for the loaded IDA SDK version.");
        return _del_hidden_range(p0);
    }

    public static int @del_idasgn(int p0)
    {
        if ((nint)_del_idasgn == 0) throw new System.EntryPointNotFoundException("'del_idasgn' is not available for the loaded IDA SDK version.");
        return _del_idasgn(p0);
    }

    public static byte @del_idc_func(byte* p0)
    {
        if ((nint)_del_idc_func == 0) throw new System.EntryPointNotFoundException("'del_idc_func' is not available for the loaded IDA SDK version.");
        return _del_idc_func(p0);
    }

    public static int @del_idcv_attr(void* p0, byte* p1)
    {
        if ((nint)_del_idcv_attr == 0) throw new System.EntryPointNotFoundException("'del_idcv_attr' is not available for the loaded IDA SDK version.");
        return _del_idcv_attr(p0, p1);
    }

    public static byte @del_item_color(ulong p0)
    {
        if ((nint)_del_item_color == 0) throw new System.EntryPointNotFoundException("'del_item_color' is not available for the loaded IDA SDK version.");
        return _del_item_color(p0);
    }

    public static byte @del_items(ulong p0, int p1, ulong p2, void* p3)
    {
        if ((nint)_del_items == 0) throw new System.EntryPointNotFoundException("'del_items' is not available for the loaded IDA SDK version.");
        return _del_items(p0, p1, p2, p3);
    }

    public static void @del_mapping(ulong p0)
    {
        if ((nint)_del_mapping == 0) throw new System.EntryPointNotFoundException("'del_mapping' is not available for the loaded IDA SDK version.");
        _del_mapping(p0);
    }

    public static byte @del_named_type(void* p0, byte* p1, int p2)
    {
        if ((nint)_del_named_type == 0) throw new System.EntryPointNotFoundException("'del_named_type' is not available for the loaded IDA SDK version.");
        return _del_named_type(p0, p1, p2);
    }

    public static void @del_node_info(ulong p0, int p1)
    {
        if ((nint)_del_node_info == 0) throw new System.EntryPointNotFoundException("'del_node_info' is not available for the loaded IDA SDK version.");
        _del_node_info(p0, p1);
    }

    public static byte @del_numbered_type(void* p0, uint p1)
    {
        if ((nint)_del_numbered_type == 0) throw new System.EntryPointNotFoundException("'del_numbered_type' is not available for the loaded IDA SDK version.");
        return _del_numbered_type(p0, p1);
    }

    public static byte @del_refinfo(ulong p0, int p1)
    {
        if ((nint)_del_refinfo == 0) throw new System.EntryPointNotFoundException("'del_refinfo' is not available for the loaded IDA SDK version.");
        return _del_refinfo(p0, p1);
    }

    public static int @del_regvar(void* p0, ulong p1, ulong p2, byte* p3)
    {
        if ((nint)_del_regvar == 0) throw new System.EntryPointNotFoundException("'del_regvar' is not available for the loaded IDA SDK version.");
        return _del_regvar(p0, p1, p2, p3);
    }

    public static byte @del_segm(ulong p0, int p1)
    {
        if ((nint)_del_segm == 0) throw new System.EntryPointNotFoundException("'del_segm' is not available for the loaded IDA SDK version.");
        return _del_segm(p0, p1);
    }

    public static void @del_segment_translations(ulong p0)
    {
        if ((nint)_del_segment_translations == 0) throw new System.EntryPointNotFoundException("'del_segment_translations' is not available for the loaded IDA SDK version.");
        _del_segment_translations(p0);
    }

    public static void @del_selector(ulong p0)
    {
        if ((nint)_del_selector == 0) throw new System.EntryPointNotFoundException("'del_selector' is not available for the loaded IDA SDK version.");
        _del_selector(p0);
    }

    public static void @del_source_linnum(ulong p0)
    {
        if ((nint)_del_source_linnum == 0) throw new System.EntryPointNotFoundException("'del_source_linnum' is not available for the loaded IDA SDK version.");
        _del_source_linnum(p0);
    }

    public static byte @del_sourcefile(ulong p0)
    {
        if ((nint)_del_sourcefile == 0) throw new System.EntryPointNotFoundException("'del_sourcefile' is not available for the loaded IDA SDK version.");
        return _del_sourcefile(p0);
    }

    public static byte @del_sreg_range(ulong p0, int p1)
    {
        if ((nint)_del_sreg_range == 0) throw new System.EntryPointNotFoundException("'del_sreg_range' is not available for the loaded IDA SDK version.");
        return _del_sreg_range(p0, p1);
    }

    public static byte @del_stkpnt(void* p0, ulong p1)
    {
        if ((nint)_del_stkpnt == 0) throw new System.EntryPointNotFoundException("'del_stkpnt' is not available for the loaded IDA SDK version.");
        return _del_stkpnt(p0, p1);
    }

    public static void @del_str_type(ulong p0)
    {
        if ((nint)_del_str_type == 0) throw new System.EntryPointNotFoundException("'del_str_type' is not available for the loaded IDA SDK version.");
        _del_str_type(p0);
    }

    public static void @del_switch_info(ulong p0)
    {
        if ((nint)_del_switch_info == 0) throw new System.EntryPointNotFoundException("'del_switch_info' is not available for the loaded IDA SDK version.");
        _del_switch_info(p0);
    }

    public static byte @del_til(byte* p0)
    {
        if ((nint)_del_til == 0) throw new System.EntryPointNotFoundException("'del_til' is not available for the loaded IDA SDK version.");
        return _del_til(p0);
    }

    public static byte @del_tinfo_attr(TypeInfo* p0, QString* p1, byte p2)
    {
        if ((nint)_del_tinfo_attr == 0) throw new System.EntryPointNotFoundException("'del_tinfo_attr' is not available for the loaded IDA SDK version.");
        return _del_tinfo_attr(p0, p1, p2);
    }

    public static void @del_tryblks(void* p0)
    {
        if ((nint)_del_tryblks == 0) throw new System.EntryPointNotFoundException("'del_tryblks' is not available for the loaded IDA SDK version.");
        _del_tryblks(p0);
    }

    public static void @del_value(ulong p0)
    {
        if ((nint)_del_value == 0) throw new System.EntryPointNotFoundException("'del_value' is not available for the loaded IDA SDK version.");
        _del_value(p0);
    }

    public static void @delete_all_xrefs_from(ulong p0, byte p1)
    {
        if ((nint)_delete_all_xrefs_from == 0) throw new System.EntryPointNotFoundException("'delete_all_xrefs_from' is not available for the loaded IDA SDK version.");
        _delete_all_xrefs_from(p0, p1);
    }

    public static void @delete_dirtree(void* p0)
    {
        if ((nint)_delete_dirtree == 0) throw new System.EntryPointNotFoundException("'delete_dirtree' is not available for the loaded IDA SDK version.");
        _delete_dirtree(p0);
    }

    public static void @delete_extra_cmts(ulong p0, int p1)
    {
        if ((nint)_delete_extra_cmts == 0) throw new System.EntryPointNotFoundException("'delete_extra_cmts' is not available for the loaded IDA SDK version.");
        _delete_extra_cmts(p0, p1);
    }

    public static byte @delete_frame_members(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_delete_frame_members == 0) throw new System.EntryPointNotFoundException("'delete_frame_members' is not available for the loaded IDA SDK version.");
        return _delete_frame_members(p0, p1, p2);
    }

    public static byte @delete_frame_members_ea(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_delete_frame_members_ea == 0) throw new System.EntryPointNotFoundException("'delete_frame_members_ea' is not available for the loaded IDA SDK version.");
        return _delete_frame_members_ea(p0, p1, p2);
    }

    public static void @delete_imports()
    {
        if ((nint)_delete_imports == 0) throw new System.EntryPointNotFoundException("'delete_imports' is not available for the loaded IDA SDK version.");
        _delete_imports();
    }

    public static void @delete_switch_table(ulong p0, void* p1)
    {
        if ((nint)_delete_switch_table == 0) throw new System.EntryPointNotFoundException("'delete_switch_table' is not available for the loaded IDA SDK version.");
        _delete_switch_table(p0, p1);
    }

    public static byte @delinf(int p0)
    {
        if ((nint)_delinf == 0) throw new System.EntryPointNotFoundException("'delinf' is not available for the loaded IDA SDK version.");
        return _delinf(p0);
    }

    public static int @demangle_name(QString* p0, byte* p1, uint p2, int p3)
    {
        if ((nint)_demangle_name == 0) throw new System.EntryPointNotFoundException("'demangle_name' is not available for the loaded IDA SDK version.");
        return _demangle_name(p0, p1, p2, p3);
    }

    public static void* @deref_idcv(void* p0, int p1)
    {
        if ((nint)_deref_idcv == 0) throw new System.EntryPointNotFoundException("'deref_idcv' is not available for the loaded IDA SDK version.");
        return _deref_idcv(p0, p1);
    }

    public static byte @deref_ptr(ulong* p0, TypeInfo* p1, ulong* p2)
    {
        if ((nint)_deref_ptr == 0) throw new System.EntryPointNotFoundException("'deref_ptr' is not available for the loaded IDA SDK version.");
        return _deref_ptr(p0, p1, p2);
    }

    public static void @deserialize_dynamic_register_set(void* p0, void* p1)
    {
        if ((nint)_deserialize_dynamic_register_set == 0) throw new System.EntryPointNotFoundException("'deserialize_dynamic_register_set' is not available for the loaded IDA SDK version.");
        _deserialize_dynamic_register_set(p0, p1);
    }

    public static void @deserialize_insn(void* p0, void* p1)
    {
        if ((nint)_deserialize_insn == 0) throw new System.EntryPointNotFoundException("'deserialize_insn' is not available for the loaded IDA SDK version.");
        _deserialize_insn(p0, p1);
    }

    public static byte @deserialize_tinfo(TypeInfo* p0, void* p1, byte** p2, byte** p3, byte** p4, byte* p5)
    {
        if ((nint)_deserialize_tinfo == 0) throw new System.EntryPointNotFoundException("'deserialize_tinfo' is not available for the loaded IDA SDK version.");
        return _deserialize_tinfo(p0, p1, p2, p3, p4, p5);
    }

    public static void @destroy_lexer(void* p0)
    {
        if ((nint)_destroy_lexer == 0) throw new System.EntryPointNotFoundException("'destroy_lexer' is not available for the loaded IDA SDK version.");
        _destroy_lexer(p0);
    }

    public static void @destroy_moddata_merge_handlers(int p0)
    {
        if ((nint)_destroy_moddata_merge_handlers == 0) throw new System.EntryPointNotFoundException("'destroy_moddata_merge_handlers' is not available for the loaded IDA SDK version.");
        _destroy_moddata_merge_handlers(p0);
    }

    public static byte @detach_custom_data_format(int p0, int p1)
    {
        if ((nint)_detach_custom_data_format == 0) throw new System.EntryPointNotFoundException("'detach_custom_data_format' is not available for the loaded IDA SDK version.");
        return _detach_custom_data_format(p0, p1);
    }

    public static byte @detach_tinfo_t(TypeInfo* p0)
    {
        if ((nint)_detach_tinfo_t == 0) throw new System.EntryPointNotFoundException("'detach_tinfo_t' is not available for the loaded IDA SDK version.");
        return _detach_tinfo_t(p0);
    }

    public static void @determine_rtl()
    {
        if ((nint)_determine_rtl == 0) throw new System.EntryPointNotFoundException("'determine_rtl' is not available for the loaded IDA SDK version.");
        _determine_rtl();
    }

    public static byte @diff_metadata(void* p0, void* p1, void* p2, uint p3)
    {
        if ((nint)_diff_metadata == 0) throw new System.EntryPointNotFoundException("'diff_metadata' is not available for the loaded IDA SDK version.");
        return _diff_metadata(p0, p1, p2, p3);
    }

    public static void @diff_source_merge_region(void* p0, void* p1, void* p2)
    {
        if ((nint)_diff_source_merge_region == 0) throw new System.EntryPointNotFoundException("'diff_source_merge_region' is not available for the loaded IDA SDK version.");
        _diff_source_merge_region(p0, p1, p2);
    }

    public static void @dirtree_add_event_handler(void* p0, void* p1)
    {
        if ((nint)_dirtree_add_event_handler == 0) throw new System.EntryPointNotFoundException("'dirtree_add_event_handler' is not available for the loaded IDA SDK version.");
        _dirtree_add_event_handler(p0, p1);
    }

    public static int @dirtree_bulk_move(void* p0, void* p1, byte* p2, int p3, void* p4, void* p5)
    {
        if ((nint)_dirtree_bulk_move == 0) throw new System.EntryPointNotFoundException("'dirtree_bulk_move' is not available for the loaded IDA SDK version.");
        return _dirtree_bulk_move(p0, p1, p2, p3, p4, p5);
    }

    public static int @dirtree_bulk_remove(void* p0, void* p1, void* p2)
    {
        if ((nint)_dirtree_bulk_remove == 0) throw new System.EntryPointNotFoundException("'dirtree_bulk_remove' is not available for the loaded IDA SDK version.");
        return _dirtree_bulk_remove(p0, p1, p2);
    }

    public static int @dirtree_change_rank(void* p0, byte* p1, nint p2)
    {
        if ((nint)_dirtree_change_rank == 0) throw new System.EntryPointNotFoundException("'dirtree_change_rank' is not available for the loaded IDA SDK version.");
        return _dirtree_change_rank(p0, p1, p2);
    }

    public static int @dirtree_chdir(void* p0, byte* p1)
    {
        if ((nint)_dirtree_chdir == 0) throw new System.EntryPointNotFoundException("'dirtree_chdir' is not available for the loaded IDA SDK version.");
        return _dirtree_chdir(p0, p1);
    }

    public static byte* @dirtree_errstr(int p0)
    {
        if ((nint)_dirtree_errstr == 0) throw new System.EntryPointNotFoundException("'dirtree_errstr' is not available for the loaded IDA SDK version.");
        return _dirtree_errstr(p0);
    }

    public static int @dirtree_find_entry(void* p0, void* p1, void* p2)
    {
        if ((nint)_dirtree_find_entry == 0) throw new System.EntryPointNotFoundException("'dirtree_find_entry' is not available for the loaded IDA SDK version.");
        return _dirtree_find_entry(p0, p1, p2);
    }

    public static byte @dirtree_findfirst(void* p0, void* p1, byte* p2)
    {
        if ((nint)_dirtree_findfirst == 0) throw new System.EntryPointNotFoundException("'dirtree_findfirst' is not available for the loaded IDA SDK version.");
        return _dirtree_findfirst(p0, p1, p2);
    }

    public static byte @dirtree_findnext(void* p0, void* p1)
    {
        if ((nint)_dirtree_findnext == 0) throw new System.EntryPointNotFoundException("'dirtree_findnext' is not available for the loaded IDA SDK version.");
        return _dirtree_findnext(p0, p1);
    }

    public static int @dirtree_fold_common_prefix(void* p0, byte* p1, byte p2)
    {
        if ((nint)_dirtree_fold_common_prefix == 0) throw new System.EntryPointNotFoundException("'dirtree_fold_common_prefix' is not available for the loaded IDA SDK version.");
        return _dirtree_fold_common_prefix(p0, p1, p2);
    }

    public static byte @dirtree_get_abspath_by_cursor(QString* p0, void* p1, void* p2, uint p3)
    {
        if ((nint)_dirtree_get_abspath_by_cursor == 0) throw new System.EntryPointNotFoundException("'dirtree_get_abspath_by_cursor' is not available for the loaded IDA SDK version.");
        return _dirtree_get_abspath_by_cursor(p0, p1, p2, p3);
    }

    public static byte @dirtree_get_abspath_by_relpath(QString* p0, void* p1, byte* p2)
    {
        if ((nint)_dirtree_get_abspath_by_relpath == 0) throw new System.EntryPointNotFoundException("'dirtree_get_abspath_by_relpath' is not available for the loaded IDA SDK version.");
        return _dirtree_get_abspath_by_relpath(p0, p1, p2);
    }

    public static nint @dirtree_get_dir_size(void* p0, ulong p1)
    {
        if ((nint)_dirtree_get_dir_size == 0) throw new System.EntryPointNotFoundException("'dirtree_get_dir_size' is not available for the loaded IDA SDK version.");
        return _dirtree_get_dir_size(p0, p1);
    }

    public static void @dirtree_get_entry_attrs(QString* p0, void* p1, void* p2)
    {
        if ((nint)_dirtree_get_entry_attrs == 0) throw new System.EntryPointNotFoundException("'dirtree_get_entry_attrs' is not available for the loaded IDA SDK version.");
        _dirtree_get_entry_attrs(p0, p1, p2);
    }

    public static byte @dirtree_get_entry_name(QString* p0, void* p1, void* p2, uint p3)
    {
        if ((nint)_dirtree_get_entry_name == 0) throw new System.EntryPointNotFoundException("'dirtree_get_entry_name' is not available for the loaded IDA SDK version.");
        return _dirtree_get_entry_name(p0, p1, p2, p3);
    }

    public static byte* @dirtree_get_id(void* p0)
    {
        if ((nint)_dirtree_get_id == 0) throw new System.EntryPointNotFoundException("'dirtree_get_id' is not available for the loaded IDA SDK version.");
        return _dirtree_get_id(p0);
    }

    public static byte* @dirtree_get_nodename(void* p0)
    {
        if ((nint)_dirtree_get_nodename == 0) throw new System.EntryPointNotFoundException("'dirtree_get_nodename' is not available for the loaded IDA SDK version.");
        return _dirtree_get_nodename(p0);
    }

    public static void @dirtree_get_parent_cursor(void* p0, void* p1, void* p2)
    {
        if ((nint)_dirtree_get_parent_cursor == 0) throw new System.EntryPointNotFoundException("'dirtree_get_parent_cursor' is not available for the loaded IDA SDK version.");
        _dirtree_get_parent_cursor(p0, p1, p2);
    }

    public static nint @dirtree_get_rank(void* p0, ulong p1, void* p2)
    {
        if ((nint)_dirtree_get_rank == 0) throw new System.EntryPointNotFoundException("'dirtree_get_rank' is not available for the loaded IDA SDK version.");
        return _dirtree_get_rank(p0, p1, p2);
    }

    public static void @dirtree_getcwd(QString* p0, void* p1)
    {
        if ((nint)_dirtree_getcwd == 0) throw new System.EntryPointNotFoundException("'dirtree_getcwd' is not available for the loaded IDA SDK version.");
        _dirtree_getcwd(p0, p1);
    }

    public static byte @dirtree_is_dir_ordered(void* p0, ulong p1)
    {
        if ((nint)_dirtree_is_dir_ordered == 0) throw new System.EntryPointNotFoundException("'dirtree_is_dir_ordered' is not available for the loaded IDA SDK version.");
        return _dirtree_is_dir_ordered(p0, p1);
    }

    public static byte @dirtree_is_orderable(void* p0)
    {
        if ((nint)_dirtree_is_orderable == 0) throw new System.EntryPointNotFoundException("'dirtree_is_orderable' is not available for the loaded IDA SDK version.");
        return _dirtree_is_orderable(p0);
    }

    public static int @dirtree_link(void* p0, byte* p1, byte p2)
    {
        if ((nint)_dirtree_link == 0) throw new System.EntryPointNotFoundException("'dirtree_link' is not available for the loaded IDA SDK version.");
        return _dirtree_link(p0, p1, p2);
    }

    public static int @dirtree_link_inode(void* p0, ulong p1, byte p2)
    {
        if ((nint)_dirtree_link_inode == 0) throw new System.EntryPointNotFoundException("'dirtree_link_inode' is not available for the loaded IDA SDK version.");
        return _dirtree_link_inode(p0, p1, p2);
    }

    public static void @dirtree_make_cursor(void* p0, void* p1, byte* p2)
    {
        if ((nint)_dirtree_make_cursor == 0) throw new System.EntryPointNotFoundException("'dirtree_make_cursor' is not available for the loaded IDA SDK version.");
        _dirtree_make_cursor(p0, p1, p2);
    }

    public static int @dirtree_mkdir(void* p0, byte* p1)
    {
        if ((nint)_dirtree_mkdir == 0) throw new System.EntryPointNotFoundException("'dirtree_mkdir' is not available for the loaded IDA SDK version.");
        return _dirtree_mkdir(p0, p1);
    }

    public static void* @dirtree_new_shadow_dirtree(void* p0)
    {
        if ((nint)_dirtree_new_shadow_dirtree == 0) throw new System.EntryPointNotFoundException("'dirtree_new_shadow_dirtree' is not available for the loaded IDA SDK version.");
        return _dirtree_new_shadow_dirtree(p0);
    }

    public static byte @dirtree_remove_event_handler(void* p0, void* p1)
    {
        if ((nint)_dirtree_remove_event_handler == 0) throw new System.EntryPointNotFoundException("'dirtree_remove_event_handler' is not available for the loaded IDA SDK version.");
        return _dirtree_remove_event_handler(p0, p1);
    }

    public static int @dirtree_rename(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_dirtree_rename == 0) throw new System.EntryPointNotFoundException("'dirtree_rename' is not available for the loaded IDA SDK version.");
        return _dirtree_rename(p0, p1, p2);
    }

    public static void @dirtree_resolve_cursor(void* p0, void* p1, void* p2)
    {
        if ((nint)_dirtree_resolve_cursor == 0) throw new System.EntryPointNotFoundException("'dirtree_resolve_cursor' is not available for the loaded IDA SDK version.");
        _dirtree_resolve_cursor(p0, p1, p2);
    }

    public static void @dirtree_resolve_path(void* p0, void* p1, byte* p2)
    {
        if ((nint)_dirtree_resolve_path == 0) throw new System.EntryPointNotFoundException("'dirtree_resolve_path' is not available for the loaded IDA SDK version.");
        _dirtree_resolve_path(p0, p1, p2);
    }

    public static int @dirtree_rmdir(void* p0, byte* p1)
    {
        if ((nint)_dirtree_rmdir == 0) throw new System.EntryPointNotFoundException("'dirtree_rmdir' is not available for the loaded IDA SDK version.");
        return _dirtree_rmdir(p0, p1);
    }

    public static void @dirtree_set_id(void* p0, byte* p1)
    {
        if ((nint)_dirtree_set_id == 0) throw new System.EntryPointNotFoundException("'dirtree_set_id' is not available for the loaded IDA SDK version.");
        _dirtree_set_id(p0, p1);
    }

    public static void @dirtree_set_nodename(void* p0, byte* p1)
    {
        if ((nint)_dirtree_set_nodename == 0) throw new System.EntryPointNotFoundException("'dirtree_set_nodename' is not available for the loaded IDA SDK version.");
        _dirtree_set_nodename(p0, p1);
    }

    public static nint @dirtree_traverse(void* p0, void* p1)
    {
        if ((nint)_dirtree_traverse == 0) throw new System.EntryPointNotFoundException("'dirtree_traverse' is not available for the loaded IDA SDK version.");
        return _dirtree_traverse(p0, p1);
    }

    public static int @disable_flags(ulong p0, ulong p1)
    {
        if ((nint)_disable_flags == 0) throw new System.EntryPointNotFoundException("'disable_flags' is not available for the loaded IDA SDK version.");
        return _disable_flags(p0, p1);
    }

    public static int @display_gdl(byte* p0)
    {
        if ((nint)_display_gdl == 0) throw new System.EntryPointNotFoundException("'display_gdl' is not available for the loaded IDA SDK version.");
        return _display_gdl(p0);
    }

    public static byte* @dstr_tinfo(TypeInfo* p0)
    {
        if ((nint)_dstr_tinfo == 0) throw new System.EntryPointNotFoundException("'dstr_tinfo' is not available for the loaded IDA SDK version.");
        return _dstr_tinfo(p0);
    }

    public static ulong @dummy_name_ea(byte* p0)
    {
        if ((nint)_dummy_name_ea == 0) throw new System.EntryPointNotFoundException("'dummy_name_ea' is not available for the loaded IDA SDK version.");
        return _dummy_name_ea(p0);
    }

    public static byte @dump_func_type_data(QString* p0, void* p1, int p2)
    {
        if ((nint)_dump_func_type_data == 0) throw new System.EntryPointNotFoundException("'dump_func_type_data' is not available for the loaded IDA SDK version.");
        return _dump_func_type_data(p0, p1, p2);
    }

    public static ulong @ea2node(ulong p0)
    {
        if ((nint)_ea2node == 0) throw new System.EntryPointNotFoundException("'ea2node' is not available for the loaded IDA SDK version.");
        return _ea2node(p0);
    }

    public static nuint @ea2str(byte* p0, nuint p1, ulong p2)
    {
        if ((nint)_ea2str == 0) throw new System.EntryPointNotFoundException("'ea2str' is not available for the loaded IDA SDK version.");
        return _ea2str(p0, p1, p2);
    }

    public static int @eadd(void* p0, void* p1, void* p2, byte p3)
    {
        if ((nint)_eadd == 0) throw new System.EntryPointNotFoundException("'eadd' is not available for the loaded IDA SDK version.");
        return _eadd(p0, p1, p2, p3);
    }

    public static void @echsize(void* p0, ulong p1)
    {
        if ((nint)_echsize == 0) throw new System.EntryPointNotFoundException("'echsize' is not available for the loaded IDA SDK version.");
        _echsize(p0, p1);
    }

    public static int @ecmp(void* p0, void* p1)
    {
        if ((nint)_ecmp == 0) throw new System.EntryPointNotFoundException("'ecmp' is not available for the loaded IDA SDK version.");
        return _ecmp(p0, p1);
    }

    public static int @ediv(void* p0, void* p1, void* p2)
    {
        if ((nint)_ediv == 0) throw new System.EntryPointNotFoundException("'ediv' is not available for the loaded IDA SDK version.");
        return _ediv(p0, p1, p2);
    }

    public static int @eetol(long* p0, void* p1, byte p2)
    {
        if ((nint)_eetol == 0) throw new System.EntryPointNotFoundException("'eetol' is not available for the loaded IDA SDK version.");
        return _eetol(p0, p1, p2);
    }

    public static int @eetol64(long* p0, void* p1, byte p2)
    {
        if ((nint)_eetol64 == 0) throw new System.EntryPointNotFoundException("'eetol64' is not available for the loaded IDA SDK version.");
        return _eetol64(p0, p1, p2);
    }

    public static int @eetol64u(ulong* p0, void* p1, byte p2)
    {
        if ((nint)_eetol64u == 0) throw new System.EntryPointNotFoundException("'eetol64u' is not available for the loaded IDA SDK version.");
        return _eetol64u(p0, p1, p2);
    }

    public static int @eldexp(void* p0, int p1, void* p2)
    {
        if ((nint)_eldexp == 0) throw new System.EntryPointNotFoundException("'eldexp' is not available for the loaded IDA SDK version.");
        return _eldexp(p0, p1, p2);
    }

    public static void @eltoe(long p0, void* p1)
    {
        if ((nint)_eltoe == 0) throw new System.EntryPointNotFoundException("'eltoe' is not available for the loaded IDA SDK version.");
        _eltoe(p0, p1);
    }

    public static void @eltoe64(long p0, void* p1)
    {
        if ((nint)_eltoe64 == 0) throw new System.EntryPointNotFoundException("'eltoe64' is not available for the loaded IDA SDK version.");
        _eltoe64(p0, p1);
    }

    public static void @eltoe64u(ulong p0, void* p1)
    {
        if ((nint)_eltoe64u == 0) throw new System.EntryPointNotFoundException("'eltoe64u' is not available for the loaded IDA SDK version.");
        _eltoe64u(p0, p1);
    }

    public static int @emul(void* p0, void* p1, void* p2)
    {
        if ((nint)_emul == 0) throw new System.EntryPointNotFoundException("'emul' is not available for the loaded IDA SDK version.");
        return _emul(p0, p1, p2);
    }

    public static byte @enable_auto(byte p0)
    {
        if ((nint)_enable_auto == 0) throw new System.EntryPointNotFoundException("'enable_auto' is not available for the loaded IDA SDK version.");
        return _enable_auto(p0);
    }

    public static void @enable_console_messages(byte p0)
    {
        if ((nint)_enable_console_messages == 0) throw new System.EntryPointNotFoundException("'enable_console_messages' is not available for the loaded IDA SDK version.");
        _enable_console_messages(p0);
    }

    public static int @enable_flags(ulong p0, ulong p1, int p2)
    {
        if ((nint)_enable_flags == 0) throw new System.EntryPointNotFoundException("'enable_flags' is not available for the loaded IDA SDK version.");
        return _enable_flags(p0, p1, p2);
    }

    public static byte @enable_numbered_types(void* p0, byte p1)
    {
        if ((nint)_enable_numbered_types == 0) throw new System.EntryPointNotFoundException("'enable_numbered_types' is not available for the loaded IDA SDK version.");
        return _enable_numbered_types(p0, p1);
    }

    public static ulong @end_ea2node(ulong p0)
    {
        if ((nint)_end_ea2node == 0) throw new System.EntryPointNotFoundException("'end_ea2node' is not available for the loaded IDA SDK version.");
        return _end_ea2node(p0);
    }

    public static void @end_type_updating(int p0)
    {
        if ((nint)_end_type_updating == 0) throw new System.EntryPointNotFoundException("'end_type_updating' is not available for the loaded IDA SDK version.");
        _end_type_updating(p0);
    }

    public static int @enum_import_names(int p0, void* p1, void* p2)
    {
        if ((nint)_enum_import_names == 0) throw new System.EntryPointNotFoundException("'enum_import_names' is not available for the loaded IDA SDK version.");
        return _enum_import_names(p0, p1, p2);
    }

    public static byte @enum_type_data_t__get_max_serial(void* p0, ulong p1)
    {
        if ((nint)_enum_type_data_t__get_max_serial == 0) throw new System.EntryPointNotFoundException("'enum_type_data_t__get_max_serial' is not available for the loaded IDA SDK version.");
        return _enum_type_data_t__get_max_serial(p0, p1);
    }

    public static int @enum_type_data_t__get_value_repr(void* p0, void* p1)
    {
        if ((nint)_enum_type_data_t__get_value_repr == 0) throw new System.EntryPointNotFoundException("'enum_type_data_t__get_value_repr' is not available for the loaded IDA SDK version.");
        return _enum_type_data_t__get_value_repr(p0, p1);
    }

    public static int @enum_type_data_t__set_value_repr(void* p0, void* p1)
    {
        if ((nint)_enum_type_data_t__set_value_repr == 0) throw new System.EntryPointNotFoundException("'enum_type_data_t__set_value_repr' is not available for the loaded IDA SDK version.");
        return _enum_type_data_t__set_value_repr(p0, p1);
    }

    public static int @enumerate_files(byte* p0, nuint p1, byte* p2, byte* p3, void* p4)
    {
        if ((nint)_enumerate_files == 0) throw new System.EntryPointNotFoundException("'enumerate_files' is not available for the loaded IDA SDK version.");
        return _enumerate_files(p0, p1, p2, p3, p4);
    }

    public static ulong @enumerate_segments_with_selector_ea(ulong p0, void* p1)
    {
        if ((nint)_enumerate_segments_with_selector_ea == 0) throw new System.EntryPointNotFoundException("'enumerate_segments_with_selector_ea' is not available for the loaded IDA SDK version.");
        return _enumerate_segments_with_selector_ea(p0, p1);
    }

    public static byte @equal_bytes(ulong p0, byte* p1, byte* p2, nuint p3, int p4)
    {
        if ((nint)_equal_bytes == 0) throw new System.EntryPointNotFoundException("'equal_bytes' is not available for the loaded IDA SDK version.");
        return _equal_bytes(p0, p1, p2, p3, p4);
    }

    public static byte @eval_expr(void* p0, ulong p1, byte* p2, QString* p3)
    {
        if ((nint)_eval_expr == 0) throw new System.EntryPointNotFoundException("'eval_expr' is not available for the loaded IDA SDK version.");
        return _eval_expr(p0, p1, p2, p3);
    }

    public static byte @eval_expr_long(long* p0, ulong p1, byte* p2, QString* p3)
    {
        if ((nint)_eval_expr_long == 0) throw new System.EntryPointNotFoundException("'eval_expr_long' is not available for the loaded IDA SDK version.");
        return _eval_expr_long(p0, p1, p2, p3);
    }

    public static byte @eval_idc_expr(void* p0, ulong p1, byte* p2, QString* p3)
    {
        if ((nint)_eval_idc_expr == 0) throw new System.EntryPointNotFoundException("'eval_idc_expr' is not available for the loaded IDA SDK version.");
        return _eval_idc_expr(p0, p1, p2, p3);
    }

    public static byte @eval_idc_snippet(void* p0, byte* p1, QString* p2, void* p3)
    {
        if ((nint)_eval_idc_snippet == 0) throw new System.EntryPointNotFoundException("'eval_idc_snippet' is not available for the loaded IDA SDK version.");
        return _eval_idc_snippet(p0, p1, p2, p3);
    }

    public static byte @exec_system_script(byte* p0, byte p1)
    {
        if ((nint)_exec_system_script == 0) throw new System.EntryPointNotFoundException("'exec_system_script' is not available for the loaded IDA SDK version.");
        return _exec_system_script(p0, p1);
    }

    public static ulong @extend_sign(ulong p0, int p1, byte p2)
    {
        if ((nint)_extend_sign == 0) throw new System.EntryPointNotFoundException("'extend_sign' is not available for the loaded IDA SDK version.");
        return _extend_sign(p0, p1, p2);
    }

    public static byte @extract_argloc(void* p0, byte** p1, byte p2)
    {
        if ((nint)_extract_argloc == 0) throw new System.EntryPointNotFoundException("'extract_argloc' is not available for the loaded IDA SDK version.");
        return _extract_argloc(p0, p1, p2);
    }

    public static void @extract_extra_cmts_from_metadata(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_extra_cmts_from_metadata == 0) throw new System.EntryPointNotFoundException("'extract_extra_cmts_from_metadata' is not available for the loaded IDA SDK version.");
        _extract_extra_cmts_from_metadata(p0, p1, p2);
    }

    public static void @extract_frame_desc_from_metadata(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_frame_desc_from_metadata == 0) throw new System.EntryPointNotFoundException("'extract_frame_desc_from_metadata' is not available for the loaded IDA SDK version.");
        _extract_frame_desc_from_metadata(p0, p1, p2);
    }

    public static void @extract_insn_cmts_from_metadata(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_insn_cmts_from_metadata == 0) throw new System.EntryPointNotFoundException("'extract_insn_cmts_from_metadata' is not available for the loaded IDA SDK version.");
        _extract_insn_cmts_from_metadata(p0, p1, p2);
    }

    public static void @extract_insn_opreprs_from_metadata(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_insn_opreprs_from_metadata == 0) throw new System.EntryPointNotFoundException("'extract_insn_opreprs_from_metadata' is not available for the loaded IDA SDK version.");
        _extract_insn_opreprs_from_metadata(p0, p1, p2);
    }

    public static void @extract_insn_opreprs_from_metadata_ex(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_insn_opreprs_from_metadata_ex == 0) throw new System.EntryPointNotFoundException("'extract_insn_opreprs_from_metadata_ex' is not available for the loaded IDA SDK version.");
        _extract_insn_opreprs_from_metadata_ex(p0, p1, p2);
    }

    public static byte @extract_module_from_archive(byte* p0, nuint p1, byte** p2, byte p3)
    {
        if ((nint)_extract_module_from_archive == 0) throw new System.EntryPointNotFoundException("'extract_module_from_archive' is not available for the loaded IDA SDK version.");
        return _extract_module_from_archive(p0, p1, p2, p3);
    }

    public static nint @extract_name(QString* p0, byte* p1, int p2)
    {
        if ((nint)_extract_name == 0) throw new System.EntryPointNotFoundException("'extract_name' is not available for the loaded IDA SDK version.");
        return _extract_name(p0, p1, p2);
    }

    public static void @extract_type_from_metadata(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_type_from_metadata == 0) throw new System.EntryPointNotFoundException("'extract_type_from_metadata' is not available for the loaded IDA SDK version.");
        _extract_type_from_metadata(p0, p1, p2);
    }

    public static void @extract_user_stkpnts_from_metadata(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_extract_user_stkpnts_from_metadata == 0) throw new System.EntryPointNotFoundException("'extract_user_stkpnts_from_metadata' is not available for the loaded IDA SDK version.");
        _extract_user_stkpnts_from_metadata(p0, p1, p2);
    }

    public static int @fc_calc_block_type(void* p0, nuint p1)
    {
        if ((nint)_fc_calc_block_type == 0) throw new System.EntryPointNotFoundException("'fc_calc_block_type' is not available for the loaded IDA SDK version.");
        return _fc_calc_block_type(p0, p1);
    }

    public static int @fc_calc_func_block_type(void* p0, nuint p1)
    {
        if ((nint)_fc_calc_func_block_type == 0) throw new System.EntryPointNotFoundException("'fc_calc_func_block_type' is not available for the loaded IDA SDK version.");
        return _fc_calc_func_block_type(p0, p1);
    }

    public static ulong @find_binary(ulong p0, ulong p1, byte* p2, int p3, int p4, int p5)
    {
        if ((nint)_find_binary == 0) throw new System.EntryPointNotFoundException("'find_binary' is not available for the loaded IDA SDK version.");
        return _find_binary(p0, p1, p2, p3, p4, p5);
    }

    public static ulong @find_byte(ulong p0, ulong p1, byte p2, int p3)
    {
        if ((nint)_find_byte == 0) throw new System.EntryPointNotFoundException("'find_byte' is not available for the loaded IDA SDK version.");
        return _find_byte(p0, p1, p2, p3);
    }

    public static ulong @find_byter(ulong p0, ulong p1, byte p2, int p3)
    {
        if ((nint)_find_byter == 0) throw new System.EntryPointNotFoundException("'find_byter' is not available for the loaded IDA SDK version.");
        return _find_byter(p0, p1, p2, p3);
    }

    public static ulong @find_code(ulong p0, int p1)
    {
        if ((nint)_find_code == 0) throw new System.EntryPointNotFoundException("'find_code' is not available for the loaded IDA SDK version.");
        return _find_code(p0, p1);
    }

    public static uint @find_custom_callcnv(byte* p0)
    {
        if ((nint)_find_custom_callcnv == 0) throw new System.EntryPointNotFoundException("'find_custom_callcnv' is not available for the loaded IDA SDK version.");
        return _find_custom_callcnv(p0);
    }

    public static int @find_custom_data_format(byte* p0)
    {
        if ((nint)_find_custom_data_format == 0) throw new System.EntryPointNotFoundException("'find_custom_data_format' is not available for the loaded IDA SDK version.");
        return _find_custom_data_format(p0);
    }

    public static int @find_custom_data_type(byte* p0)
    {
        if ((nint)_find_custom_data_type == 0) throw new System.EntryPointNotFoundException("'find_custom_data_type' is not available for the loaded IDA SDK version.");
        return _find_custom_data_type(p0);
    }

    public static ushort @find_custom_fixup(byte* p0)
    {
        if ((nint)_find_custom_fixup == 0) throw new System.EntryPointNotFoundException("'find_custom_fixup' is not available for the loaded IDA SDK version.");
        return _find_custom_fixup(p0);
    }

    public static int @find_custom_refinfo(byte* p0)
    {
        if ((nint)_find_custom_refinfo == 0) throw new System.EntryPointNotFoundException("'find_custom_refinfo' is not available for the loaded IDA SDK version.");
        return _find_custom_refinfo(p0);
    }

    public static ulong @find_data(ulong p0, int p1)
    {
        if ((nint)_find_data == 0) throw new System.EntryPointNotFoundException("'find_data' is not available for the loaded IDA SDK version.");
        return _find_data(p0, p1);
    }

    public static ulong @find_defined(ulong p0, int p1)
    {
        if ((nint)_find_defined == 0) throw new System.EntryPointNotFoundException("'find_defined' is not available for the loaded IDA SDK version.");
        return _find_defined(p0, p1);
    }

    public static ulong @find_defjump_from_table(ulong p0, void* p1)
    {
        if ((nint)_find_defjump_from_table == 0) throw new System.EntryPointNotFoundException("'find_defjump_from_table' is not available for the loaded IDA SDK version.");
        return _find_defjump_from_table(p0, p1);
    }

    public static ulong @find_error(ulong p0, int p1, int* p2)
    {
        if ((nint)_find_error == 0) throw new System.EntryPointNotFoundException("'find_error' is not available for the loaded IDA SDK version.");
        return _find_error(p0, p1, p2);
    }

    public static void* @find_extlang(void* p0, int p1)
    {
        if ((nint)_find_extlang == 0) throw new System.EntryPointNotFoundException("'find_extlang' is not available for the loaded IDA SDK version.");
        return _find_extlang(p0, p1);
    }

    public static ulong @find_free_chunk(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_find_free_chunk == 0) throw new System.EntryPointNotFoundException("'find_free_chunk' is not available for the loaded IDA SDK version.");
        return _find_free_chunk(p0, p1, p2);
    }

    public static ulong @find_free_selector()
    {
        if ((nint)_find_free_selector == 0) throw new System.EntryPointNotFoundException("'find_free_selector' is not available for the loaded IDA SDK version.");
        return _find_free_selector();
    }

    public static int @find_func_bounds(void* p0, int p1)
    {
        if ((nint)_find_func_bounds == 0) throw new System.EntryPointNotFoundException("'find_func_bounds' is not available for the loaded IDA SDK version.");
        return _find_func_bounds(p0, p1);
    }

    public static nint @find_func_regvar(void* p0, ulong p1, ulong p2, ulong p3, byte* p4, byte* p5)
    {
        if ((nint)_find_func_regvar == 0) throw new System.EntryPointNotFoundException("'find_func_regvar' is not available for the loaded IDA SDK version.");
        return _find_func_regvar(p0, p1, p2, p3, p4, p5);
    }

    public static int @find_function_bounds(void* p0, int p1)
    {
        if ((nint)_find_function_bounds == 0) throw new System.EntryPointNotFoundException("'find_function_bounds' is not available for the loaded IDA SDK version.");
        return _find_function_bounds(p0, p1);
    }

    public static void* @find_idc_class(byte* p0)
    {
        if ((nint)_find_idc_class == 0) throw new System.EntryPointNotFoundException("'find_idc_class' is not available for the loaded IDA SDK version.");
        return _find_idc_class(p0);
    }

    public static byte @find_idc_func(QString* p0, byte* p1, int p2)
    {
        if ((nint)_find_idc_func == 0) throw new System.EntryPointNotFoundException("'find_idc_func' is not available for the loaded IDA SDK version.");
        return _find_idc_func(p0, p1, p2);
    }

    public static void* @find_idc_gvar(byte* p0)
    {
        if ((nint)_find_idc_gvar == 0) throw new System.EntryPointNotFoundException("'find_idc_gvar' is not available for the loaded IDA SDK version.");
        return _find_idc_gvar(p0);
    }

    public static ulong @find_imm(ulong p0, int p1, ulong p2, int* p3)
    {
        if ((nint)_find_imm == 0) throw new System.EntryPointNotFoundException("'find_imm' is not available for the loaded IDA SDK version.");
        return _find_imm(p0, p1, p2, p3);
    }

    public static void* @find_ioport(void* p0, ulong p1)
    {
        if ((nint)_find_ioport == 0) throw new System.EntryPointNotFoundException("'find_ioport' is not available for the loaded IDA SDK version.");
        return _find_ioport(p0, p1);
    }

    public static void* @find_ioport_bit(void* p0, ulong p1, nuint p2)
    {
        if ((nint)_find_ioport_bit == 0) throw new System.EntryPointNotFoundException("'find_ioport_bit' is not available for the loaded IDA SDK version.");
        return _find_ioport_bit(p0, p1, p2);
    }

    public static byte @find_jtable_size(void* p0)
    {
        if ((nint)_find_jtable_size == 0) throw new System.EntryPointNotFoundException("'find_jtable_size' is not available for the loaded IDA SDK version.");
        return _find_jtable_size(p0);
    }

    public static ulong @find_not_func(ulong p0, int p1)
    {
        if ((nint)_find_not_func == 0) throw new System.EntryPointNotFoundException("'find_not_func' is not available for the loaded IDA SDK version.");
        return _find_not_func(p0, p1);
    }

    public static ulong @find_notype(ulong p0, int p1, int* p2)
    {
        if ((nint)_find_notype == 0) throw new System.EntryPointNotFoundException("'find_notype' is not available for the loaded IDA SDK version.");
        return _find_notype(p0, p1, p2);
    }

    public static void* @find_plugin(byte* p0, byte p1)
    {
        if ((nint)_find_plugin == 0) throw new System.EntryPointNotFoundException("'find_plugin' is not available for the loaded IDA SDK version.");
        return _find_plugin(p0, p1);
    }

    public static ulong @find_reg_access(void* p0, ulong p1, ulong p2, byte* p3, int p4)
    {
        if ((nint)_find_reg_access == 0) throw new System.EntryPointNotFoundException("'find_reg_access' is not available for the loaded IDA SDK version.");
        return _find_reg_access(p0, p1, p2, p3, p4);
    }

    public static int @find_reg_value(ulong* p0, ulong p1, int p2)
    {
        if ((nint)_find_reg_value == 0) throw new System.EntryPointNotFoundException("'find_reg_value' is not available for the loaded IDA SDK version.");
        return _find_reg_value(p0, p1, p2);
    }

    public static byte @find_reg_value_info(void* p0, ulong p1, int p2, int p3)
    {
        if ((nint)_find_reg_value_info == 0) throw new System.EntryPointNotFoundException("'find_reg_value_info' is not available for the loaded IDA SDK version.");
        return _find_reg_value_info(p0, p1, p2, p3);
    }

    public static byte @find_regname_value_info(void* p0, ulong p1, byte* p2, int p3)
    {
        if ((nint)_find_regname_value_info == 0) throw new System.EntryPointNotFoundException("'find_regname_value_info' is not available for the loaded IDA SDK version.");
        return _find_regname_value_info(p0, p1, p2, p3);
    }

    public static void* @find_regvar(void* p0, ulong p1, ulong p2, byte* p3, byte* p4)
    {
        if ((nint)_find_regvar == 0) throw new System.EntryPointNotFoundException("'find_regvar' is not available for the loaded IDA SDK version.");
        return _find_regvar(p0, p1, p2, p3, p4);
    }

    public static ulong @find_selector(ulong p0)
    {
        if ((nint)_find_selector == 0) throw new System.EntryPointNotFoundException("'find_selector' is not available for the loaded IDA SDK version.");
        return _find_selector(p0);
    }

    public static int @find_sp_value(long* p0, ulong p1, int p2)
    {
        if ((nint)_find_sp_value == 0) throw new System.EntryPointNotFoundException("'find_sp_value' is not available for the loaded IDA SDK version.");
        return _find_sp_value(p0, p1, p2);
    }

    public static ulong @find_suspop(ulong p0, int p1, int* p2)
    {
        if ((nint)_find_suspop == 0) throw new System.EntryPointNotFoundException("'find_suspop' is not available for the loaded IDA SDK version.");
        return _find_suspop(p0, p1, p2);
    }

    public static ulong @find_syseh(ulong p0)
    {
        if ((nint)_find_syseh == 0) throw new System.EntryPointNotFoundException("'find_syseh' is not available for the loaded IDA SDK version.");
        return _find_syseh(p0);
    }

    public static ulong @find_text(ulong p0, int p1, int p2, byte* p3, int p4)
    {
        if ((nint)_find_text == 0) throw new System.EntryPointNotFoundException("'find_text' is not available for the loaded IDA SDK version.");
        return _find_text(p0, p1, p2, p3, p4);
    }

    public static int @find_tinfo_udt_member(void* p0, ulong p1, int p2)
    {
        if ((nint)_find_tinfo_udt_member == 0) throw new System.EntryPointNotFoundException("'find_tinfo_udt_member' is not available for the loaded IDA SDK version.");
        return _find_tinfo_udt_member(p0, p1, p2);
    }

    public static ulong @find_unknown(ulong p0, int p1)
    {
        if ((nint)_find_unknown == 0) throw new System.EntryPointNotFoundException("'find_unknown' is not available for the loaded IDA SDK version.");
        return _find_unknown(p0, p1);
    }

    public static byte* @first_idcv_attr(void* p0)
    {
        if ((nint)_first_idcv_attr == 0) throw new System.EntryPointNotFoundException("'first_idcv_attr' is not available for the loaded IDA SDK version.");
        return _first_idcv_attr(p0);
    }

    public static byte* @first_named_type(void* p0, int p1)
    {
        if ((nint)_first_named_type == 0) throw new System.EntryPointNotFoundException("'first_named_type' is not available for the loaded IDA SDK version.");
        return _first_named_type(p0, p1);
    }

    public static int @flush_buffers()
    {
        if ((nint)_flush_buffers == 0) throw new System.EntryPointNotFoundException("'flush_buffers' is not available for the loaded IDA SDK version.");
        return _flush_buffers();
    }

    public static void* @fopenA(byte* p0)
    {
        if ((nint)_fopenA == 0) throw new System.EntryPointNotFoundException("'fopenA' is not available for the loaded IDA SDK version.");
        return _fopenA(p0);
    }

    public static void* @fopenM(byte* p0)
    {
        if ((nint)_fopenM == 0) throw new System.EntryPointNotFoundException("'fopenM' is not available for the loaded IDA SDK version.");
        return _fopenM(p0);
    }

    public static void* @fopenRB(byte* p0)
    {
        if ((nint)_fopenRB == 0) throw new System.EntryPointNotFoundException("'fopenRB' is not available for the loaded IDA SDK version.");
        return _fopenRB(p0);
    }

    public static void* @fopenRT(byte* p0)
    {
        if ((nint)_fopenRT == 0) throw new System.EntryPointNotFoundException("'fopenRT' is not available for the loaded IDA SDK version.");
        return _fopenRT(p0);
    }

    public static void* @fopenWB(byte* p0)
    {
        if ((nint)_fopenWB == 0) throw new System.EntryPointNotFoundException("'fopenWB' is not available for the loaded IDA SDK version.");
        return _fopenWB(p0);
    }

    public static void* @fopenWT(byte* p0)
    {
        if ((nint)_fopenWT == 0) throw new System.EntryPointNotFoundException("'fopenWT' is not available for the loaded IDA SDK version.");
        return _fopenWT(p0);
    }

    public static int @for_all_arglocs(void* p0, void* p1, int p2, int p3)
    {
        if ((nint)_for_all_arglocs == 0) throw new System.EntryPointNotFoundException("'for_all_arglocs' is not available for the loaded IDA SDK version.");
        return _for_all_arglocs(p0, p1, p2, p3);
    }

    public static nint @for_all_extlangs(void* p0, byte p1)
    {
        if ((nint)_for_all_extlangs == 0) throw new System.EntryPointNotFoundException("'for_all_extlangs' is not available for the loaded IDA SDK version.");
        return _for_all_extlangs(p0, p1);
    }

    public static byte @forget_problem(byte p0, ulong p1)
    {
        if ((nint)_forget_problem == 0) throw new System.EntryPointNotFoundException("'forget_problem' is not available for the loaded IDA SDK version.");
        return _forget_problem(p0, p1);
    }

    public static byte @format_cdata(void* p0, void* p1, TypeInfo* p2, void* p3, void* p4)
    {
        if ((nint)_format_cdata == 0) throw new System.EntryPointNotFoundException("'format_cdata' is not available for the loaded IDA SDK version.");
        return _format_cdata(p0, p1, p2, p3, p4);
    }

    public static byte @format_charlit(QString* p0, byte** p1, nuint p2, uint p3, int p4)
    {
        if ((nint)_format_charlit == 0) throw new System.EntryPointNotFoundException("'format_charlit' is not available for the loaded IDA SDK version.");
        return _format_charlit(p0, p1, p2, p3, p4);
    }

    public static byte @format_timestamp(byte* p0, nuint p1, ulong p2, uint p3)
    {
        if ((nint)_format_timestamp == 0) throw new System.EntryPointNotFoundException("'format_timestamp' is not available for the loaded IDA SDK version.");
        return _format_timestamp(p0, p1, p2, p3);
    }

    public static int @freadbytes(void* p0, void* p1, int p2, int p3)
    {
        if ((nint)_freadbytes == 0) throw new System.EntryPointNotFoundException("'freadbytes' is not available for the loaded IDA SDK version.");
        return _freadbytes(p0, p1, p2, p3);
    }

    public static void @free_debug_event(void* p0)
    {
        if ((nint)_free_debug_event == 0) throw new System.EntryPointNotFoundException("'free_debug_event' is not available for the loaded IDA SDK version.");
        _free_debug_event(p0);
    }

    public static void @free_dll(void* p0)
    {
        if ((nint)_free_dll == 0) throw new System.EntryPointNotFoundException("'free_dll' is not available for the loaded IDA SDK version.");
        _free_dll(p0);
    }

    public static void @free_idcv(void* p0)
    {
        if ((nint)_free_idcv == 0) throw new System.EntryPointNotFoundException("'free_idcv' is not available for the loaded IDA SDK version.");
        _free_idcv(p0);
    }

    public static void @free_loaders_list(void* p0)
    {
        if ((nint)_free_loaders_list == 0) throw new System.EntryPointNotFoundException("'free_loaders_list' is not available for the loaded IDA SDK version.");
        _free_loaders_list(p0);
    }

    public static void @free_regarg(void* p0)
    {
        if ((nint)_free_regarg == 0) throw new System.EntryPointNotFoundException("'free_regarg' is not available for the loaded IDA SDK version.");
        _free_regarg(p0);
    }

    public static void @free_regvar(void* p0)
    {
        if ((nint)_free_regvar == 0) throw new System.EntryPointNotFoundException("'free_regvar' is not available for the loaded IDA SDK version.");
        _free_regvar(p0);
    }

    public static void @free_til(void* p0)
    {
        if ((nint)_free_til == 0) throw new System.EntryPointNotFoundException("'free_til' is not available for the loaded IDA SDK version.");
        _free_til(p0);
    }

    public static byte @func_does_return(ulong p0)
    {
        if ((nint)_func_does_return == 0) throw new System.EntryPointNotFoundException("'func_does_return' is not available for the loaded IDA SDK version.");
        return _func_does_return(p0);
    }

    public static byte @func_has_stkframe_hole(ulong p0, void* p1)
    {
        if ((nint)_func_has_stkframe_hole == 0) throw new System.EntryPointNotFoundException("'func_has_stkframe_hole' is not available for the loaded IDA SDK version.");
        return _func_has_stkframe_hole(p0, p1);
    }

    public static byte @func_item_iterator_decode_preceding_insn(void* p0, void* p1, byte* p2, void* p3)
    {
        if ((nint)_func_item_iterator_decode_preceding_insn == 0) throw new System.EntryPointNotFoundException("'func_item_iterator_decode_preceding_insn' is not available for the loaded IDA SDK version.");
        return _func_item_iterator_decode_preceding_insn(p0, p1, p2, p3);
    }

    public static byte @func_item_iterator_decode_prev_insn(void* p0, void* p1)
    {
        if ((nint)_func_item_iterator_decode_prev_insn == 0) throw new System.EntryPointNotFoundException("'func_item_iterator_decode_prev_insn' is not available for the loaded IDA SDK version.");
        return _func_item_iterator_decode_prev_insn(p0, p1);
    }

    public static byte @func_item_iterator_next(void* p0, void* p1, void* p2)
    {
        if ((nint)_func_item_iterator_next == 0) throw new System.EntryPointNotFoundException("'func_item_iterator_next' is not available for the loaded IDA SDK version.");
        return _func_item_iterator_next(p0, p1, p2);
    }

    public static byte @func_item_iterator_prev(void* p0, void* p1, void* p2)
    {
        if ((nint)_func_item_iterator_prev == 0) throw new System.EntryPointNotFoundException("'func_item_iterator_prev' is not available for the loaded IDA SDK version.");
        return _func_item_iterator_prev(p0, p1, p2);
    }

    public static byte @func_item_iterator_succ(void* p0, void* p1, void* p2)
    {
        if ((nint)_func_item_iterator_succ == 0) throw new System.EntryPointNotFoundException("'func_item_iterator_succ' is not available for the loaded IDA SDK version.");
        return _func_item_iterator_succ(p0, p1, p2);
    }

    public static byte @func_parent_iterator_set(void* p0, void* p1)
    {
        if ((nint)_func_parent_iterator_set == 0) throw new System.EntryPointNotFoundException("'func_parent_iterator_set' is not available for the loaded IDA SDK version.");
        return _func_parent_iterator_set(p0, p1);
    }

    public static byte @func_tail_iterator_set(void* p0, void* p1, ulong p2)
    {
        if ((nint)_func_tail_iterator_set == 0) throw new System.EntryPointNotFoundException("'func_tail_iterator_set' is not available for the loaded IDA SDK version.");
        return _func_tail_iterator_set(p0, p1, p2);
    }

    public static byte @func_tail_iterator_set_ea(void* p0, ulong p1)
    {
        if ((nint)_func_tail_iterator_set_ea == 0) throw new System.EntryPointNotFoundException("'func_tail_iterator_set_ea' is not available for the loaded IDA SDK version.");
        return _func_tail_iterator_set_ea(p0, p1);
    }

    public static byte @function_item_iterator_decode_preceding_insn(void* p0, void* p1, byte* p2, void* p3)
    {
        if ((nint)_function_item_iterator_decode_preceding_insn == 0) throw new System.EntryPointNotFoundException("'function_item_iterator_decode_preceding_insn' is not available for the loaded IDA SDK version.");
        return _function_item_iterator_decode_preceding_insn(p0, p1, p2, p3);
    }

    public static byte @function_item_iterator_decode_prev_insn(void* p0, void* p1)
    {
        if ((nint)_function_item_iterator_decode_prev_insn == 0) throw new System.EntryPointNotFoundException("'function_item_iterator_decode_prev_insn' is not available for the loaded IDA SDK version.");
        return _function_item_iterator_decode_prev_insn(p0, p1);
    }

    public static byte @function_item_iterator_next(void* p0, void* p1, void* p2)
    {
        if ((nint)_function_item_iterator_next == 0) throw new System.EntryPointNotFoundException("'function_item_iterator_next' is not available for the loaded IDA SDK version.");
        return _function_item_iterator_next(p0, p1, p2);
    }

    public static byte @function_item_iterator_prev(void* p0, void* p1, void* p2)
    {
        if ((nint)_function_item_iterator_prev == 0) throw new System.EntryPointNotFoundException("'function_item_iterator_prev' is not available for the loaded IDA SDK version.");
        return _function_item_iterator_prev(p0, p1, p2);
    }

    public static byte @function_item_iterator_succ(void* p0, void* p1, void* p2)
    {
        if ((nint)_function_item_iterator_succ == 0) throw new System.EntryPointNotFoundException("'function_item_iterator_succ' is not available for the loaded IDA SDK version.");
        return _function_item_iterator_succ(p0, p1, p2);
    }

    public static byte @function_parent_iterator_first(void* p0)
    {
        if ((nint)_function_parent_iterator_first == 0) throw new System.EntryPointNotFoundException("'function_parent_iterator_first' is not available for the loaded IDA SDK version.");
        return _function_parent_iterator_first(p0);
    }

    public static byte @function_parent_iterator_last(void* p0)
    {
        if ((nint)_function_parent_iterator_last == 0) throw new System.EntryPointNotFoundException("'function_parent_iterator_last' is not available for the loaded IDA SDK version.");
        return _function_parent_iterator_last(p0);
    }

    public static byte @function_parent_iterator_next(void* p0)
    {
        if ((nint)_function_parent_iterator_next == 0) throw new System.EntryPointNotFoundException("'function_parent_iterator_next' is not available for the loaded IDA SDK version.");
        return _function_parent_iterator_next(p0);
    }

    public static ulong @function_parent_iterator_parent(void* p0)
    {
        if ((nint)_function_parent_iterator_parent == 0) throw new System.EntryPointNotFoundException("'function_parent_iterator_parent' is not available for the loaded IDA SDK version.");
        return _function_parent_iterator_parent(p0);
    }

    public static byte @function_parent_iterator_prev(void* p0)
    {
        if ((nint)_function_parent_iterator_prev == 0) throw new System.EntryPointNotFoundException("'function_parent_iterator_prev' is not available for the loaded IDA SDK version.");
        return _function_parent_iterator_prev(p0);
    }

    public static byte @function_parent_iterator_set(void* p0, ulong p1)
    {
        if ((nint)_function_parent_iterator_set == 0) throw new System.EntryPointNotFoundException("'function_parent_iterator_set' is not available for the loaded IDA SDK version.");
        return _function_parent_iterator_set(p0, p1);
    }

    public static void @function_tail_iterator_chunk(void* p0, void* p1)
    {
        if ((nint)_function_tail_iterator_chunk == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_chunk' is not available for the loaded IDA SDK version.");
        _function_tail_iterator_chunk(p0, p1);
    }

    public static byte @function_tail_iterator_first(void* p0)
    {
        if ((nint)_function_tail_iterator_first == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_first' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_first(p0);
    }

    public static byte @function_tail_iterator_last(void* p0)
    {
        if ((nint)_function_tail_iterator_last == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_last' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_last(p0);
    }

    public static byte @function_tail_iterator_main(void* p0)
    {
        if ((nint)_function_tail_iterator_main == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_main' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_main(p0);
    }

    public static byte @function_tail_iterator_next(void* p0)
    {
        if ((nint)_function_tail_iterator_next == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_next' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_next(p0);
    }

    public static byte @function_tail_iterator_prev(void* p0)
    {
        if ((nint)_function_tail_iterator_prev == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_prev' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_prev(p0);
    }

    public static byte @function_tail_iterator_set(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_function_tail_iterator_set == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_set' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_set(p0, p1, p2);
    }

    public static byte @function_tail_iterator_set_ea(void* p0, ulong p1)
    {
        if ((nint)_function_tail_iterator_set_ea == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_set_ea' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_set_ea(p0, p1);
    }

    public static byte @function_tail_iterator_set_range(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_function_tail_iterator_set_range == 0) throw new System.EntryPointNotFoundException("'function_tail_iterator_set_range' is not available for the loaded IDA SDK version.");
        return _function_tail_iterator_set_range(p0, p1, p2);
    }

    public static int @fwritebytes(void* p0, void* p1, int p2, int p3)
    {
        if ((nint)_fwritebytes == 0) throw new System.EntryPointNotFoundException("'fwritebytes' is not available for the loaded IDA SDK version.");
        return _fwritebytes(p0, p1, p2, p3);
    }

    public static byte @gen_complex_call_chart(byte* p0, byte* p1, byte* p2, ulong p3, ulong p4, int p5, int p6)
    {
        if ((nint)_gen_complex_call_chart == 0) throw new System.EntryPointNotFoundException("'gen_complex_call_chart' is not available for the loaded IDA SDK version.");
        return _gen_complex_call_chart(p0, p1, p2, p3, p4, p5, p6);
    }

    public static byte @gen_decorate_name(QString* p0, byte* p1, byte p2, uint p3, TypeInfo* p4)
    {
        if ((nint)_gen_decorate_name == 0) throw new System.EntryPointNotFoundException("'gen_decorate_name' is not available for the loaded IDA SDK version.");
        return _gen_decorate_name(p0, p1, p2, p3, p4);
    }

    public static int @gen_exe_file(void* p0)
    {
        if ((nint)_gen_exe_file == 0) throw new System.EntryPointNotFoundException("'gen_exe_file' is not available for the loaded IDA SDK version.");
        return _gen_exe_file(p0);
    }

    public static int @gen_file(int p0, void* p1, ulong p2, ulong p3, int p4)
    {
        if ((nint)_gen_file == 0) throw new System.EntryPointNotFoundException("'gen_file' is not available for the loaded IDA SDK version.");
        return _gen_file(p0, p1, p2, p3, p4);
    }

    public static void @gen_fix_fixups(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_gen_fix_fixups == 0) throw new System.EntryPointNotFoundException("'gen_fix_fixups' is not available for the loaded IDA SDK version.");
        _gen_fix_fixups(p0, p1, p2);
    }

    public static byte @gen_flow_graph(byte* p0, byte* p1, void* p2, ulong p3, ulong p4, int p5)
    {
        if ((nint)_gen_flow_graph == 0) throw new System.EntryPointNotFoundException("'gen_flow_graph' is not available for the loaded IDA SDK version.");
        return _gen_flow_graph(p0, p1, p2, p3, p4, p5);
    }

    public static byte @gen_flow_graph_ea(byte* p0, byte* p1, ulong p2, ulong p3, ulong p4, int p5)
    {
        if ((nint)_gen_flow_graph_ea == 0) throw new System.EntryPointNotFoundException("'gen_flow_graph_ea' is not available for the loaded IDA SDK version.");
        return _gen_flow_graph_ea(p0, p1, p2, p3, p4, p5);
    }

    public static void @gen_gdl(void* p0, byte* p1)
    {
        if ((nint)_gen_gdl == 0) throw new System.EntryPointNotFoundException("'gen_gdl' is not available for the loaded IDA SDK version.");
        _gen_gdl(p0, p1);
    }

    public static byte @gen_rand_buf(void* p0, nuint p1)
    {
        if ((nint)_gen_rand_buf == 0) throw new System.EntryPointNotFoundException("'gen_rand_buf' is not available for the loaded IDA SDK version.");
        return _gen_rand_buf(p0, p1);
    }

    public static byte @gen_simple_call_chart(byte* p0, byte* p1, byte* p2, int p3)
    {
        if ((nint)_gen_simple_call_chart == 0) throw new System.EntryPointNotFoundException("'gen_simple_call_chart' is not available for the loaded IDA SDK version.");
        return _gen_simple_call_chart(p0, p1, p2, p3);
    }

    public static void @gen_use_arg_tinfos(void* p0, ulong p1, void* p2, void* p3)
    {
        if ((nint)_gen_use_arg_tinfos == 0) throw new System.EntryPointNotFoundException("'gen_use_arg_tinfos' is not available for the loaded IDA SDK version.");
        _gen_use_arg_tinfos(p0, p1, p2, p3);
    }

    public static byte @generate_disasm_line(QString* p0, ulong p1, int p2)
    {
        if ((nint)_generate_disasm_line == 0) throw new System.EntryPointNotFoundException("'generate_disasm_line' is not available for the loaded IDA SDK version.");
        return _generate_disasm_line(p0, p1, p2);
    }

    public static int @generate_disassembly(void* p0, int* p1, ulong p2, int p3, int p4)
    {
        if ((nint)_generate_disassembly == 0) throw new System.EntryPointNotFoundException("'generate_disassembly' is not available for the loaded IDA SDK version.");
        return _generate_disassembly(p0, p1, p2, p3, p4);
    }

    public static uint @get_16bit(ulong p0)
    {
        if ((nint)_get_16bit == 0) throw new System.EntryPointNotFoundException("'get_16bit' is not available for the loaded IDA SDK version.");
        return _get_16bit(p0);
    }

    public static uint @get_32bit(ulong p0)
    {
        if ((nint)_get_32bit == 0) throw new System.EntryPointNotFoundException("'get_32bit' is not available for the loaded IDA SDK version.");
        return _get_32bit(p0);
    }

    public static ulong @get_64bit(ulong p0)
    {
        if ((nint)_get_64bit == 0) throw new System.EntryPointNotFoundException("'get_64bit' is not available for the loaded IDA SDK version.");
        return _get_64bit(p0);
    }

    public static nint @get_abi_name(QString* p0)
    {
        if ((nint)_get_abi_name == 0) throw new System.EntryPointNotFoundException("'get_abi_name' is not available for the loaded IDA SDK version.");
        return _get_abi_name(p0);
    }

    public static uint @get_aflags(ulong p0)
    {
        if ((nint)_get_aflags == 0) throw new System.EntryPointNotFoundException("'get_aflags' is not available for the loaded IDA SDK version.");
        return _get_aflags(p0);
    }

    public static uint @get_alias_target(void* p0, uint p1)
    {
        if ((nint)_get_alias_target == 0) throw new System.EntryPointNotFoundException("'get_alias_target' is not available for the loaded IDA SDK version.");
        return _get_alias_target(p0, p1);
    }

    public static byte @get_arg_addrs(void* p0, ulong p1)
    {
        if ((nint)_get_arg_addrs == 0) throw new System.EntryPointNotFoundException("'get_arg_addrs' is not available for the loaded IDA SDK version.");
        return _get_arg_addrs(p0, p1);
    }

    public static nint @get_array_parameters(void* p0, ulong p1)
    {
        if ((nint)_get_array_parameters == 0) throw new System.EntryPointNotFoundException("'get_array_parameters' is not available for the loaded IDA SDK version.");
        return _get_array_parameters(p0, p1);
    }

    public static void* @get_ash()
    {
        if ((nint)_get_ash == 0) throw new System.EntryPointNotFoundException("'get_ash' is not available for the loaded IDA SDK version.");
        return _get_ash();
    }

    public static byte @get_auto_display(AutoDisplay* p0)
    {
        if ((nint)_get_auto_display == 0) throw new System.EntryPointNotFoundException("'get_auto_display' is not available for the loaded IDA SDK version.");
        return _get_auto_display(p0);
    }

    public static int @get_auto_state()
    {
        if ((nint)_get_auto_state == 0) throw new System.EntryPointNotFoundException("'get_auto_state' is not available for the loaded IDA SDK version.");
        return _get_auto_state();
    }

    public static int @get_available_core_count()
    {
        if ((nint)_get_available_core_count == 0) throw new System.EntryPointNotFoundException("'get_available_core_count' is not available for the loaded IDA SDK version.");
        return _get_available_core_count();
    }

    public static int @get_basic_file_type(void* p0)
    {
        if ((nint)_get_basic_file_type == 0) throw new System.EntryPointNotFoundException("'get_basic_file_type' is not available for the loaded IDA SDK version.");
        return _get_basic_file_type(p0);
    }

    public static void @get_builtin_widgets_state(void* p0)
    {
        if ((nint)_get_builtin_widgets_state == 0) throw new System.EntryPointNotFoundException("'get_builtin_widgets_state' is not available for the loaded IDA SDK version.");
        _get_builtin_widgets_state(p0);
    }

    public static byte @get_byte(ulong p0)
    {
        if ((nint)_get_byte == 0) throw new System.EntryPointNotFoundException("'get_byte' is not available for the loaded IDA SDK version.");
        return _get_byte(p0);
    }

    public static nint @get_bytes(void* p0, nint p1, ulong p2, int p3, void* p4)
    {
        if ((nint)_get_bytes == 0) throw new System.EntryPointNotFoundException("'get_bytes' is not available for the loaded IDA SDK version.");
        return _get_bytes(p0, p1, p2, p3, p4);
    }

    public static nint @get_cmt(QString* p0, ulong p1, byte p2)
    {
        if ((nint)_get_cmt == 0) throw new System.EntryPointNotFoundException("'get_cmt' is not available for the loaded IDA SDK version.");
        return _get_cmt(p0, p1, p2);
    }

    public static byte* @get_compiler_abbr(byte p0)
    {
        if ((nint)_get_compiler_abbr == 0) throw new System.EntryPointNotFoundException("'get_compiler_abbr' is not available for the loaded IDA SDK version.");
        return _get_compiler_abbr(p0);
    }

    public static byte* @get_compiler_name(byte p0)
    {
        if ((nint)_get_compiler_name == 0) throw new System.EntryPointNotFoundException("'get_compiler_name' is not available for the loaded IDA SDK version.");
        return _get_compiler_name(p0);
    }

    public static void @get_compilers(void* p0, void* p1, void* p2)
    {
        if ((nint)_get_compilers == 0) throw new System.EntryPointNotFoundException("'get_compilers' is not available for the loaded IDA SDK version.");
        _get_compilers(p0, p1, p2);
    }

    public static byte @get_config_value(void* p0, byte* p1)
    {
        if ((nint)_get_config_value == 0) throw new System.EntryPointNotFoundException("'get_config_value' is not available for the loaded IDA SDK version.");
        return _get_config_value(p0, p1);
    }

    public static byte @get_cp_validity(int p0, uint p1, uint p2)
    {
        if ((nint)_get_cp_validity == 0) throw new System.EntryPointNotFoundException("'get_cp_validity' is not available for the loaded IDA SDK version.");
        return _get_cp_validity(p0, p1, p2);
    }

    public static void* @get_current_extlang()
    {
        if ((nint)_get_current_extlang == 0) throw new System.EntryPointNotFoundException("'get_current_extlang' is not available for the loaded IDA SDK version.");
        return _get_current_extlang();
    }

    public static int @get_current_idasgn()
    {
        if ((nint)_get_current_idasgn == 0) throw new System.EntryPointNotFoundException("'get_current_idasgn' is not available for the loaded IDA SDK version.");
        return _get_current_idasgn();
    }

    public static void* @get_custom_callcnv(uint p0)
    {
        if ((nint)_get_custom_callcnv == 0) throw new System.EntryPointNotFoundException("'get_custom_callcnv' is not available for the loaded IDA SDK version.");
        return _get_custom_callcnv(p0);
    }

    public static nuint @get_custom_callcnvs(void* p0, void* p1)
    {
        if ((nint)_get_custom_callcnvs == 0) throw new System.EntryPointNotFoundException("'get_custom_callcnvs' is not available for the loaded IDA SDK version.");
        return _get_custom_callcnvs(p0, p1);
    }

    public static void* @get_custom_data_format(int p0)
    {
        if ((nint)_get_custom_data_format == 0) throw new System.EntryPointNotFoundException("'get_custom_data_format' is not available for the loaded IDA SDK version.");
        return _get_custom_data_format(p0);
    }

    public static int @get_custom_data_formats(void* p0, int p1)
    {
        if ((nint)_get_custom_data_formats == 0) throw new System.EntryPointNotFoundException("'get_custom_data_formats' is not available for the loaded IDA SDK version.");
        return _get_custom_data_formats(p0, p1);
    }

    public static void* @get_custom_data_type(int p0)
    {
        if ((nint)_get_custom_data_type == 0) throw new System.EntryPointNotFoundException("'get_custom_data_type' is not available for the loaded IDA SDK version.");
        return _get_custom_data_type(p0);
    }

    public static int @get_custom_data_type_ids(void* p0, ulong p1)
    {
        if ((nint)_get_custom_data_type_ids == 0) throw new System.EntryPointNotFoundException("'get_custom_data_type_ids' is not available for the loaded IDA SDK version.");
        return _get_custom_data_type_ids(p0, p1);
    }

    public static int @get_custom_data_types(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_get_custom_data_types == 0) throw new System.EntryPointNotFoundException("'get_custom_data_types' is not available for the loaded IDA SDK version.");
        return _get_custom_data_types(p0, p1, p2);
    }

    public static void* @get_custom_refinfo(int p0)
    {
        if ((nint)_get_custom_refinfo == 0) throw new System.EntryPointNotFoundException("'get_custom_refinfo' is not available for the loaded IDA SDK version.");
        return _get_custom_refinfo(p0);
    }

    public static ulong @get_data_elsize(ulong p0, ulong p1, void* p2)
    {
        if ((nint)_get_data_elsize == 0) throw new System.EntryPointNotFoundException("'get_data_elsize' is not available for the loaded IDA SDK version.");
        return _get_data_elsize(p0, p1, p2);
    }

    public static byte @get_data_value(ulong* p0, ulong p1, ulong p2)
    {
        if ((nint)_get_data_value == 0) throw new System.EntryPointNotFoundException("'get_data_value' is not available for the loaded IDA SDK version.");
        return _get_data_value(p0, p1, p2);
    }

    public static byte @get_db_byte(ulong p0)
    {
        if ((nint)_get_db_byte == 0) throw new System.EntryPointNotFoundException("'get_db_byte' is not available for the loaded IDA SDK version.");
        return _get_db_byte(p0);
    }

    public static nint @get_dbctx_id()
    {
        if ((nint)_get_dbctx_id == 0) throw new System.EntryPointNotFoundException("'get_dbctx_id' is not available for the loaded IDA SDK version.");
        return _get_dbctx_id();
    }

    public static nuint @get_dbctx_qty()
    {
        if ((nint)_get_dbctx_qty == 0) throw new System.EntryPointNotFoundException("'get_dbctx_qty' is not available for the loaded IDA SDK version.");
        return _get_dbctx_qty();
    }

    public static byte @get_dbg_byte(uint* p0, ulong p1)
    {
        if ((nint)_get_dbg_byte == 0) throw new System.EntryPointNotFoundException("'get_dbg_byte' is not available for the loaded IDA SDK version.");
        return _get_dbg_byte(p0, p1);
    }

    public static nint @get_debug_name(QString* p0, ulong* p1, int p2)
    {
        if ((nint)_get_debug_name == 0) throw new System.EntryPointNotFoundException("'get_debug_name' is not available for the loaded IDA SDK version.");
        return _get_debug_name(p0, p1, p2);
    }

    public static ulong @get_debug_name_ea(byte* p0)
    {
        if ((nint)_get_debug_name_ea == 0) throw new System.EntryPointNotFoundException("'get_debug_name_ea' is not available for the loaded IDA SDK version.");
        return _get_debug_name_ea(p0);
    }

    public static void @get_debug_names(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_get_debug_names == 0) throw new System.EntryPointNotFoundException("'get_debug_names' is not available for the loaded IDA SDK version.");
        _get_debug_names(p0, p1, p2);
    }

    public static nuint @get_debugger_plugins(void* p0)
    {
        if ((nint)_get_debugger_plugins == 0) throw new System.EntryPointNotFoundException("'get_debugger_plugins' is not available for the loaded IDA SDK version.");
        return _get_debugger_plugins(p0);
    }

    public static int @get_default_encoding_idx(int p0)
    {
        if ((nint)_get_default_encoding_idx == 0) throw new System.EntryPointNotFoundException("'get_default_encoding_idx' is not available for the loaded IDA SDK version.");
        return _get_default_encoding_idx(p0);
    }

    public static int @get_default_radix()
    {
        if ((nint)_get_default_radix == 0) throw new System.EntryPointNotFoundException("'get_default_radix' is not available for the loaded IDA SDK version.");
        return _get_default_radix();
    }

    public static byte @get_default_reftype(ulong p0)
    {
        if ((nint)_get_default_reftype == 0) throw new System.EntryPointNotFoundException("'get_default_reftype' is not available for the loaded IDA SDK version.");
        return _get_default_reftype(p0);
    }

    public static ulong @get_dirty_infos()
    {
        if ((nint)_get_dirty_infos == 0) throw new System.EntryPointNotFoundException("'get_dirty_infos' is not available for the loaded IDA SDK version.");
        return _get_dirty_infos();
    }

    public static byte @get_dtype_by_size(ulong p0)
    {
        if ((nint)_get_dtype_by_size == 0) throw new System.EntryPointNotFoundException("'get_dtype_by_size' is not available for the loaded IDA SDK version.");
        return _get_dtype_by_size(p0);
    }

    public static ulong @get_dtype_flag(byte p0)
    {
        if ((nint)_get_dtype_flag == 0) throw new System.EntryPointNotFoundException("'get_dtype_flag' is not available for the loaded IDA SDK version.");
        return _get_dtype_flag(p0);
    }

    public static nuint @get_dtype_size(byte p0)
    {
        if ((nint)_get_dtype_size == 0) throw new System.EntryPointNotFoundException("'get_dtype_size' is not available for the loaded IDA SDK version.");
        return _get_dtype_size(p0);
    }

    public static uint @get_dword(ulong p0)
    {
        if ((nint)_get_dword == 0) throw new System.EntryPointNotFoundException("'get_dword' is not available for the loaded IDA SDK version.");
        return _get_dword(p0);
    }

    public static nint @get_ea_diffpos_name(QString* p0, ulong p1)
    {
        if ((nint)_get_ea_diffpos_name == 0) throw new System.EntryPointNotFoundException("'get_ea_diffpos_name' is not available for the loaded IDA SDK version.");
        return _get_ea_diffpos_name(p0, p1);
    }

    public static nint @get_ea_name(QString* p0, ulong p1, int p2, void* p3)
    {
        if ((nint)_get_ea_name == 0) throw new System.EntryPointNotFoundException("'get_ea_name' is not available for the loaded IDA SDK version.");
        return _get_ea_name(p0, p1, p2, p3);
    }

    public static void* @get_eah()
    {
        if ((nint)_get_eah == 0) throw new System.EntryPointNotFoundException("'get_eah' is not available for the loaded IDA SDK version.");
        return _get_eah();
    }

    public static nint @get_edm_by_tid(TypeInfo* p0, void* p1, ulong p2)
    {
        if ((nint)_get_edm_by_tid == 0) throw new System.EntryPointNotFoundException("'get_edm_by_tid' is not available for the loaded IDA SDK version.");
        return _get_edm_by_tid(p0, p1, p2);
    }

    public static long @get_effective_spd(void* p0, ulong p1)
    {
        if ((nint)_get_effective_spd == 0) throw new System.EntryPointNotFoundException("'get_effective_spd' is not available for the loaded IDA SDK version.");
        return _get_effective_spd(p0, p1);
    }

    public static byte* @get_elf_debug_file_directory()
    {
        if ((nint)_get_elf_debug_file_directory == 0) throw new System.EntryPointNotFoundException("'get_elf_debug_file_directory' is not available for the loaded IDA SDK version.");
        return _get_elf_debug_file_directory();
    }

    public static int @get_encoding_bpu(int p0)
    {
        if ((nint)_get_encoding_bpu == 0) throw new System.EntryPointNotFoundException("'get_encoding_bpu' is not available for the loaded IDA SDK version.");
        return _get_encoding_bpu(p0);
    }

    public static int @get_encoding_bpu_by_name(byte* p0)
    {
        if ((nint)_get_encoding_bpu_by_name == 0) throw new System.EntryPointNotFoundException("'get_encoding_bpu_by_name' is not available for the loaded IDA SDK version.");
        return _get_encoding_bpu_by_name(p0);
    }

    public static byte* @get_encoding_name(int p0)
    {
        if ((nint)_get_encoding_name == 0) throw new System.EntryPointNotFoundException("'get_encoding_name' is not available for the loaded IDA SDK version.");
        return _get_encoding_name(p0);
    }

    public static int @get_encoding_qty()
    {
        if ((nint)_get_encoding_qty == 0) throw new System.EntryPointNotFoundException("'get_encoding_qty' is not available for the loaded IDA SDK version.");
        return _get_encoding_qty();
    }

    public static ulong @get_entry(ulong p0)
    {
        if ((nint)_get_entry == 0) throw new System.EntryPointNotFoundException("'get_entry' is not available for the loaded IDA SDK version.");
        return _get_entry(p0);
    }

    public static nint @get_entry_forwarder(QString* p0, ulong p1)
    {
        if ((nint)_get_entry_forwarder == 0) throw new System.EntryPointNotFoundException("'get_entry_forwarder' is not available for the loaded IDA SDK version.");
        return _get_entry_forwarder(p0, p1);
    }

    public static nint @get_entry_name(QString* p0, ulong p1)
    {
        if ((nint)_get_entry_name == 0) throw new System.EntryPointNotFoundException("'get_entry_name' is not available for the loaded IDA SDK version.");
        return _get_entry_name(p0, p1);
    }

    public static ulong @get_entry_ordinal(nuint p0)
    {
        if ((nint)_get_entry_ordinal == 0) throw new System.EntryPointNotFoundException("'get_entry_ordinal' is not available for the loaded IDA SDK version.");
        return _get_entry_ordinal(p0);
    }

    public static nuint @get_entry_qty()
    {
        if ((nint)_get_entry_qty == 0) throw new System.EntryPointNotFoundException("'get_entry_qty' is not available for the loaded IDA SDK version.");
        return _get_entry_qty();
    }

    public static ulong @get_enum_id(byte* p0, ulong p1, int p2)
    {
        if ((nint)_get_enum_id == 0) throw new System.EntryPointNotFoundException("'get_enum_id' is not available for the loaded IDA SDK version.");
        return _get_enum_id(p0, p1, p2);
    }

    public static byte @get_enum_member_expr(QString* p0, TypeInfo* p1, int p2, ulong p3)
    {
        if ((nint)_get_enum_member_expr == 0) throw new System.EntryPointNotFoundException("'get_enum_member_expr' is not available for the loaded IDA SDK version.");
        return _get_enum_member_expr(p0, p1, p2, p3);
    }

    public static byte* @get_errdesc(byte* p0, int p1)
    {
        if ((nint)_get_errdesc == 0) throw new System.EntryPointNotFoundException("'get_errdesc' is not available for the loaded IDA SDK version.");
        return _get_errdesc(p0, p1);
    }

    public static nuint @get_error_data(int p0)
    {
        if ((nint)_get_error_data == 0) throw new System.EntryPointNotFoundException("'get_error_data' is not available for the loaded IDA SDK version.");
        return _get_error_data(p0);
    }

    public static byte* @get_error_string(int p0)
    {
        if ((nint)_get_error_string == 0) throw new System.EntryPointNotFoundException("'get_error_string' is not available for the loaded IDA SDK version.");
        return _get_error_string(p0);
    }

    public static nint @get_extra_cmt(QString* p0, ulong p1, int p2)
    {
        if ((nint)_get_extra_cmt == 0) throw new System.EntryPointNotFoundException("'get_extra_cmt' is not available for the loaded IDA SDK version.");
        return _get_extra_cmt(p0, p1, p2);
    }

    public static void* @get_fchunk(ulong p0)
    {
        if ((nint)_get_fchunk == 0) throw new System.EntryPointNotFoundException("'get_fchunk' is not available for the loaded IDA SDK version.");
        return _get_fchunk(p0);
    }

    public static ulong @get_fchunk_ea_by_num(int p0)
    {
        if ((nint)_get_fchunk_ea_by_num == 0) throw new System.EntryPointNotFoundException("'get_fchunk_ea_by_num' is not available for the loaded IDA SDK version.");
        return _get_fchunk_ea_by_num(p0);
    }

    public static byte @get_fchunk_info(void* p0, ulong p1)
    {
        if ((nint)_get_fchunk_info == 0) throw new System.EntryPointNotFoundException("'get_fchunk_info' is not available for the loaded IDA SDK version.");
        return _get_fchunk_info(p0, p1);
    }

    public static int @get_fchunk_num(ulong p0)
    {
        if ((nint)_get_fchunk_num == 0) throw new System.EntryPointNotFoundException("'get_fchunk_num' is not available for the loaded IDA SDK version.");
        return _get_fchunk_num(p0);
    }

    public static nuint @get_fchunk_qty()
    {
        if ((nint)_get_fchunk_qty == 0) throw new System.EntryPointNotFoundException("'get_fchunk_qty' is not available for the loaded IDA SDK version.");
        return _get_fchunk_qty();
    }

    public static ulong @get_fchunk_start(ulong p0)
    {
        if ((nint)_get_fchunk_start == 0) throw new System.EntryPointNotFoundException("'get_fchunk_start' is not available for the loaded IDA SDK version.");
        return _get_fchunk_start(p0);
    }

    public static byte* @get_file_ext(byte* p0)
    {
        if ((nint)_get_file_ext == 0) throw new System.EntryPointNotFoundException("'get_file_ext' is not available for the loaded IDA SDK version.");
        return _get_file_ext(p0);
    }

    public static nuint @get_file_type_name(byte* p0, nuint p1)
    {
        if ((nint)_get_file_type_name == 0) throw new System.EntryPointNotFoundException("'get_file_type_name' is not available for the loaded IDA SDK version.");
        return _get_file_type_name(p0, p1);
    }

    public static ulong @get_first_cref_from(ulong p0)
    {
        if ((nint)_get_first_cref_from == 0) throw new System.EntryPointNotFoundException("'get_first_cref_from' is not available for the loaded IDA SDK version.");
        return _get_first_cref_from(p0);
    }

    public static ulong @get_first_cref_to(ulong p0)
    {
        if ((nint)_get_first_cref_to == 0) throw new System.EntryPointNotFoundException("'get_first_cref_to' is not available for the loaded IDA SDK version.");
        return _get_first_cref_to(p0);
    }

    public static ulong @get_first_dref_from(ulong p0)
    {
        if ((nint)_get_first_dref_from == 0) throw new System.EntryPointNotFoundException("'get_first_dref_from' is not available for the loaded IDA SDK version.");
        return _get_first_dref_from(p0);
    }

    public static ulong @get_first_dref_to(ulong p0)
    {
        if ((nint)_get_first_dref_to == 0) throw new System.EntryPointNotFoundException("'get_first_dref_to' is not available for the loaded IDA SDK version.");
        return _get_first_dref_to(p0);
    }

    public static ulong @get_first_fcref_from(ulong p0)
    {
        if ((nint)_get_first_fcref_from == 0) throw new System.EntryPointNotFoundException("'get_first_fcref_from' is not available for the loaded IDA SDK version.");
        return _get_first_fcref_from(p0);
    }

    public static ulong @get_first_fcref_to(ulong p0)
    {
        if ((nint)_get_first_fcref_to == 0) throw new System.EntryPointNotFoundException("'get_first_fcref_to' is not available for the loaded IDA SDK version.");
        return _get_first_fcref_to(p0);
    }

    public static ulong @get_first_fixup_ea()
    {
        if ((nint)_get_first_fixup_ea == 0) throw new System.EntryPointNotFoundException("'get_first_fixup_ea' is not available for the loaded IDA SDK version.");
        return _get_first_fixup_ea();
    }

    public static int @get_first_free_extra_cmtidx(ulong p0, int p1)
    {
        if ((nint)_get_first_free_extra_cmtidx == 0) throw new System.EntryPointNotFoundException("'get_first_free_extra_cmtidx' is not available for the loaded IDA SDK version.");
        return _get_first_free_extra_cmtidx(p0, p1);
    }

    public static void* @get_first_hidden_range()
    {
        if ((nint)_get_first_hidden_range == 0) throw new System.EntryPointNotFoundException("'get_first_hidden_range' is not available for the loaded IDA SDK version.");
        return _get_first_hidden_range();
    }

    public static ulong @get_first_hidden_range_ea()
    {
        if ((nint)_get_first_hidden_range_ea == 0) throw new System.EntryPointNotFoundException("'get_first_hidden_range_ea' is not available for the loaded IDA SDK version.");
        return _get_first_hidden_range_ea();
    }

    public static void* @get_first_seg()
    {
        if ((nint)_get_first_seg == 0) throw new System.EntryPointNotFoundException("'get_first_seg' is not available for the loaded IDA SDK version.");
        return _get_first_seg();
    }

    public static ulong @get_first_segment_ea()
    {
        if ((nint)_get_first_segment_ea == 0) throw new System.EntryPointNotFoundException("'get_first_segment_ea' is not available for the loaded IDA SDK version.");
        return _get_first_segment_ea();
    }

    public static byte @get_fixup(void* p0, ulong p1)
    {
        if ((nint)_get_fixup == 0) throw new System.EntryPointNotFoundException("'get_fixup' is not available for the loaded IDA SDK version.");
        return _get_fixup(p0, p1);
    }

    public static byte* @get_fixup_desc(QString* p0, ulong p1, void* p2)
    {
        if ((nint)_get_fixup_desc == 0) throw new System.EntryPointNotFoundException("'get_fixup_desc' is not available for the loaded IDA SDK version.");
        return _get_fixup_desc(p0, p1, p2);
    }

    public static void* @get_fixup_handler(ushort p0)
    {
        if ((nint)_get_fixup_handler == 0) throw new System.EntryPointNotFoundException("'get_fixup_handler' is not available for the loaded IDA SDK version.");
        return _get_fixup_handler(p0);
    }

    public static ulong @get_fixup_value(ulong p0, ushort p1)
    {
        if ((nint)_get_fixup_value == 0) throw new System.EntryPointNotFoundException("'get_fixup_value' is not available for the loaded IDA SDK version.");
        return _get_fixup_value(p0, p1);
    }

    public static byte @get_fixups(void* p0, ulong p1, ulong p2)
    {
        if ((nint)_get_fixups == 0) throw new System.EntryPointNotFoundException("'get_fixups' is not available for the loaded IDA SDK version.");
        return _get_fixups(p0, p1, p2);
    }

    public static ulong @get_flags_by_size(nuint p0)
    {
        if ((nint)_get_flags_by_size == 0) throw new System.EntryPointNotFoundException("'get_flags_by_size' is not available for the loaded IDA SDK version.");
        return _get_flags_by_size(p0);
    }

    public static ulong @get_flags_ex(ulong p0, int p1)
    {
        if ((nint)_get_flags_ex == 0) throw new System.EntryPointNotFoundException("'get_flags_ex' is not available for the loaded IDA SDK version.");
        return _get_flags_ex(p0, p1);
    }

    public static nint @get_forced_operand(QString* p0, ulong p1, int p2)
    {
        if ((nint)_get_forced_operand == 0) throw new System.EntryPointNotFoundException("'get_forced_operand' is not available for the loaded IDA SDK version.");
        return _get_forced_operand(p0, p1, p2);
    }

    public static int @get_fpvalue_kind(void* p0, ushort p1)
    {
        if ((nint)_get_fpvalue_kind == 0) throw new System.EntryPointNotFoundException("'get_fpvalue_kind' is not available for the loaded IDA SDK version.");
        return _get_fpvalue_kind(p0, p1);
    }

    public static void @get_frame_part(void* p0, void* p1, int p2)
    {
        if ((nint)_get_frame_part == 0) throw new System.EntryPointNotFoundException("'get_frame_part' is not available for the loaded IDA SDK version.");
        _get_frame_part(p0, p1, p2);
    }

    public static byte @get_frame_part_ea(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_frame_part_ea == 0) throw new System.EntryPointNotFoundException("'get_frame_part_ea' is not available for the loaded IDA SDK version.");
        return _get_frame_part_ea(p0, p1, p2);
    }

    public static int @get_frame_retsize(void* p0)
    {
        if ((nint)_get_frame_retsize == 0) throw new System.EntryPointNotFoundException("'get_frame_retsize' is not available for the loaded IDA SDK version.");
        return _get_frame_retsize(p0);
    }

    public static int @get_frame_retsize_ea(ulong p0)
    {
        if ((nint)_get_frame_retsize_ea == 0) throw new System.EntryPointNotFoundException("'get_frame_retsize_ea' is not available for the loaded IDA SDK version.");
        return _get_frame_retsize_ea(p0);
    }

    public static ulong @get_frame_size(void* p0)
    {
        if ((nint)_get_frame_size == 0) throw new System.EntryPointNotFoundException("'get_frame_size' is not available for the loaded IDA SDK version.");
        return _get_frame_size(p0);
    }

    public static ulong @get_frame_size_ea(ulong p0)
    {
        if ((nint)_get_frame_size_ea == 0) throw new System.EntryPointNotFoundException("'get_frame_size_ea' is not available for the loaded IDA SDK version.");
        return _get_frame_size_ea(p0);
    }

    public static nint @get_frame_var(TypeInfo* p0, long* p1, void* p2, void* p3, long p4)
    {
        if ((nint)_get_frame_var == 0) throw new System.EntryPointNotFoundException("'get_frame_var' is not available for the loaded IDA SDK version.");
        return _get_frame_var(p0, p1, p2, p3, p4);
    }

    public static ulong @get_free_disk_space(byte* p0)
    {
        if ((nint)_get_free_disk_space == 0) throw new System.EntryPointNotFoundException("'get_free_disk_space' is not available for the loaded IDA SDK version.");
        return _get_free_disk_space(p0);
    }

    public static void* @get_func(ulong p0)
    {
        if ((nint)_get_func == 0) throw new System.EntryPointNotFoundException("'get_func' is not available for the loaded IDA SDK version.");
        return _get_func(p0);
    }

    public static int @get_func_bitness(void* p0)
    {
        if ((nint)_get_func_bitness == 0) throw new System.EntryPointNotFoundException("'get_func_bitness' is not available for the loaded IDA SDK version.");
        return _get_func_bitness(p0);
    }

    public static int @get_func_bitness_ea(ulong p0)
    {
        if ((nint)_get_func_bitness_ea == 0) throw new System.EntryPointNotFoundException("'get_func_bitness_ea' is not available for the loaded IDA SDK version.");
        return _get_func_bitness_ea(p0);
    }

    public static int @get_func_chunknum(void* p0, ulong p1)
    {
        if ((nint)_get_func_chunknum == 0) throw new System.EntryPointNotFoundException("'get_func_chunknum' is not available for the loaded IDA SDK version.");
        return _get_func_chunknum(p0, p1);
    }

    public static int @get_func_chunknum_ea(ulong p0, ulong p1)
    {
        if ((nint)_get_func_chunknum_ea == 0) throw new System.EntryPointNotFoundException("'get_func_chunknum_ea' is not available for the loaded IDA SDK version.");
        return _get_func_chunknum_ea(p0, p1);
    }

    public static nint @get_func_cmt(QString* p0, void* p1, byte p2)
    {
        if ((nint)_get_func_cmt == 0) throw new System.EntryPointNotFoundException("'get_func_cmt' is not available for the loaded IDA SDK version.");
        return _get_func_cmt(p0, p1, p2);
    }

    public static nint @get_func_cmt_ea(QString* p0, ulong p1, byte p2)
    {
        if ((nint)_get_func_cmt_ea == 0) throw new System.EntryPointNotFoundException("'get_func_cmt_ea' is not available for the loaded IDA SDK version.");
        return _get_func_cmt_ea(p0, p1, p2);
    }

    public static ulong @get_func_ea_by_num(nuint p0)
    {
        if ((nint)_get_func_ea_by_num == 0) throw new System.EntryPointNotFoundException("'get_func_ea_by_num' is not available for the loaded IDA SDK version.");
        return _get_func_ea_by_num(p0);
    }

    public static long @get_func_effective_spd(ulong p0, ulong p1)
    {
        if ((nint)_get_func_effective_spd == 0) throw new System.EntryPointNotFoundException("'get_func_effective_spd' is not available for the loaded IDA SDK version.");
        return _get_func_effective_spd(p0, p1);
    }

    public static byte @get_func_entry_info(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_func_entry_info == 0) throw new System.EntryPointNotFoundException("'get_func_entry_info' is not available for the loaded IDA SDK version.");
        return _get_func_entry_info(p0, p1, p2);
    }

    public static byte @get_func_entry_info_by_num(void* p0, nuint p1, int p2)
    {
        if ((nint)_get_func_entry_info_by_num == 0) throw new System.EntryPointNotFoundException("'get_func_entry_info_by_num' is not available for the loaded IDA SDK version.");
        return _get_func_entry_info_by_num(p0, p1, p2);
    }

    public static ulong @get_func_flags(ulong p0)
    {
        if ((nint)_get_func_flags == 0) throw new System.EntryPointNotFoundException("'get_func_flags' is not available for the loaded IDA SDK version.");
        return _get_func_flags(p0);
    }

    public static byte @get_func_frame(TypeInfo* p0, void* p1)
    {
        if ((nint)_get_func_frame == 0) throw new System.EntryPointNotFoundException("'get_func_frame' is not available for the loaded IDA SDK version.");
        return _get_func_frame(p0, p1);
    }

    public static byte @get_func_frame_ea(TypeInfo* p0, ulong p1)
    {
        if ((nint)_get_func_frame_ea == 0) throw new System.EntryPointNotFoundException("'get_func_frame_ea' is not available for the loaded IDA SDK version.");
        return _get_func_frame_ea(p0, p1);
    }

    public static nuint @get_func_llabel_qty(ulong p0)
    {
        if ((nint)_get_func_llabel_qty == 0) throw new System.EntryPointNotFoundException("'get_func_llabel_qty' is not available for the loaded IDA SDK version.");
        return _get_func_llabel_qty(p0);
    }

    public static byte @get_func_llabels(void* p0, ulong p1)
    {
        if ((nint)_get_func_llabels == 0) throw new System.EntryPointNotFoundException("'get_func_llabels' is not available for the loaded IDA SDK version.");
        return _get_func_llabels(p0, p1);
    }

    public static nint @get_func_name(QString* p0, ulong p1)
    {
        if ((nint)_get_func_name == 0) throw new System.EntryPointNotFoundException("'get_func_name' is not available for the loaded IDA SDK version.");
        return _get_func_name(p0, p1);
    }

    public static int @get_func_num(ulong p0)
    {
        if ((nint)_get_func_num == 0) throw new System.EntryPointNotFoundException("'get_func_num' is not available for the loaded IDA SDK version.");
        return _get_func_num(p0);
    }

    public static nuint @get_func_qty()
    {
        if ((nint)_get_func_qty == 0) throw new System.EntryPointNotFoundException("'get_func_qty' is not available for the loaded IDA SDK version.");
        return _get_func_qty();
    }

    public static ulong @get_func_ranges(void* p0, void* p1)
    {
        if ((nint)_get_func_ranges == 0) throw new System.EntryPointNotFoundException("'get_func_ranges' is not available for the loaded IDA SDK version.");
        return _get_func_ranges(p0, p1);
    }

    public static ulong @get_func_ranges_ea(void* p0, ulong p1)
    {
        if ((nint)_get_func_ranges_ea == 0) throw new System.EntryPointNotFoundException("'get_func_ranges_ea' is not available for the loaded IDA SDK version.");
        return _get_func_ranges_ea(p0, p1);
    }

    public static byte @get_func_regarg(void* p0, ulong p1, nuint p2)
    {
        if ((nint)_get_func_regarg == 0) throw new System.EntryPointNotFoundException("'get_func_regarg' is not available for the loaded IDA SDK version.");
        return _get_func_regarg(p0, p1, p2);
    }

    public static nuint @get_func_regarg_qty(ulong p0)
    {
        if ((nint)_get_func_regarg_qty == 0) throw new System.EntryPointNotFoundException("'get_func_regarg_qty' is not available for the loaded IDA SDK version.");
        return _get_func_regarg_qty(p0);
    }

    public static byte @get_func_regargs(void* p0, ulong p1)
    {
        if ((nint)_get_func_regargs == 0) throw new System.EntryPointNotFoundException("'get_func_regargs' is not available for the loaded IDA SDK version.");
        return _get_func_regargs(p0, p1);
    }

    public static byte @get_func_regvar(void* p0, ulong p1, nint p2)
    {
        if ((nint)_get_func_regvar == 0) throw new System.EntryPointNotFoundException("'get_func_regvar' is not available for the loaded IDA SDK version.");
        return _get_func_regvar(p0, p1, p2);
    }

    public static nuint @get_func_regvar_qty(ulong p0)
    {
        if ((nint)_get_func_regvar_qty == 0) throw new System.EntryPointNotFoundException("'get_func_regvar_qty' is not available for the loaded IDA SDK version.");
        return _get_func_regvar_qty(p0);
    }

    public static byte @get_func_regvars(void* p0, ulong p1)
    {
        if ((nint)_get_func_regvars == 0) throw new System.EntryPointNotFoundException("'get_func_regvars' is not available for the loaded IDA SDK version.");
        return _get_func_regvars(p0, p1);
    }

    public static long @get_func_sp_delta(ulong p0, ulong p1)
    {
        if ((nint)_get_func_sp_delta == 0) throw new System.EntryPointNotFoundException("'get_func_sp_delta' is not available for the loaded IDA SDK version.");
        return _get_func_sp_delta(p0, p1);
    }

    public static long @get_func_spd(ulong p0, ulong p1)
    {
        if ((nint)_get_func_spd == 0) throw new System.EntryPointNotFoundException("'get_func_spd' is not available for the loaded IDA SDK version.");
        return _get_func_spd(p0, p1);
    }

    public static ulong @get_func_start(ulong p0)
    {
        if ((nint)_get_func_start == 0) throw new System.EntryPointNotFoundException("'get_func_start' is not available for the loaded IDA SDK version.");
        return _get_func_start(p0);
    }

    public static nuint @get_func_stkpnt_qty(ulong p0)
    {
        if ((nint)_get_func_stkpnt_qty == 0) throw new System.EntryPointNotFoundException("'get_func_stkpnt_qty' is not available for the loaded IDA SDK version.");
        return _get_func_stkpnt_qty(p0);
    }

    public static byte @get_func_stkpnts(void* p0, ulong p1)
    {
        if ((nint)_get_func_stkpnts == 0) throw new System.EntryPointNotFoundException("'get_func_stkpnts' is not available for the loaded IDA SDK version.");
        return _get_func_stkpnts(p0, p1);
    }

    public static byte @get_func_tail_info(void* p0, ulong p1)
    {
        if ((nint)_get_func_tail_info == 0) throw new System.EntryPointNotFoundException("'get_func_tail_info' is not available for the loaded IDA SDK version.");
        return _get_func_tail_info(p0, p1);
    }

    public static nuint @get_func_tail_qty(ulong p0)
    {
        if ((nint)_get_func_tail_qty == 0) throw new System.EntryPointNotFoundException("'get_func_tail_qty' is not available for the loaded IDA SDK version.");
        return _get_func_tail_qty(p0);
    }

    public static byte @get_func_tails(void* p0, ulong p1)
    {
        if ((nint)_get_func_tails == 0) throw new System.EntryPointNotFoundException("'get_func_tails' is not available for the loaded IDA SDK version.");
        return _get_func_tails(p0, p1);
    }

    public static ulong @get_group_selector(ulong p0)
    {
        if ((nint)_get_group_selector == 0) throw new System.EntryPointNotFoundException("'get_group_selector' is not available for the loaded IDA SDK version.");
        return _get_group_selector(p0);
    }

    public static void* @get_hexdsp()
    {
        if ((nint)_get_hexdsp == 0) throw new System.EntryPointNotFoundException("'get_hexdsp' is not available for the loaded IDA SDK version.");
        return _get_hexdsp();
    }

    public static void* @get_hidden_range(ulong p0)
    {
        if ((nint)_get_hidden_range == 0) throw new System.EntryPointNotFoundException("'get_hidden_range' is not available for the loaded IDA SDK version.");
        return _get_hidden_range(p0);
    }

    public static byte @get_hidden_range_info(void* p0, ulong p1)
    {
        if ((nint)_get_hidden_range_info == 0) throw new System.EntryPointNotFoundException("'get_hidden_range_info' is not available for the loaded IDA SDK version.");
        return _get_hidden_range_info(p0, p1);
    }

    public static byte @get_hidden_range_info_by_num(void* p0, int p1)
    {
        if ((nint)_get_hidden_range_info_by_num == 0) throw new System.EntryPointNotFoundException("'get_hidden_range_info_by_num' is not available for the loaded IDA SDK version.");
        return _get_hidden_range_info_by_num(p0, p1);
    }

    public static int @get_hidden_range_num(ulong p0)
    {
        if ((nint)_get_hidden_range_num == 0) throw new System.EntryPointNotFoundException("'get_hidden_range_num' is not available for the loaded IDA SDK version.");
        return _get_hidden_range_num(p0);
    }

    public static int @get_hidden_range_qty()
    {
        if ((nint)_get_hidden_range_qty == 0) throw new System.EntryPointNotFoundException("'get_hidden_range_qty' is not available for the loaded IDA SDK version.");
        return _get_hidden_range_qty();
    }

    public static int @get_ida_subdirs(void* p0, byte* p1, int p2)
    {
        if ((nint)_get_ida_subdirs == 0) throw new System.EntryPointNotFoundException("'get_ida_subdirs' is not available for the loaded IDA SDK version.");
        return _get_ida_subdirs(p0, p1, p2);
    }

    public static byte @get_idainfo_by_type(nuint* p0, ulong* p1, void* p2, TypeInfo* p3, nuint* p4)
    {
        if ((nint)_get_idainfo_by_type == 0) throw new System.EntryPointNotFoundException("'get_idainfo_by_type' is not available for the loaded IDA SDK version.");
        return _get_idainfo_by_type(p0, p1, p2, p3, p4);
    }

    public static byte @get_idainfo_by_udm(ulong* p0, void* p1, void* p2, ulong p3)
    {
        if ((nint)_get_idainfo_by_udm == 0) throw new System.EntryPointNotFoundException("'get_idainfo_by_udm' is not available for the loaded IDA SDK version.");
        return _get_idainfo_by_udm(p0, p1, p2, p3);
    }

    public static int @get_idasgn_desc(QString* p0, QString* p1, int p2)
    {
        if ((nint)_get_idasgn_desc == 0) throw new System.EntryPointNotFoundException("'get_idasgn_desc' is not available for the loaded IDA SDK version.");
        return _get_idasgn_desc(p0, p1, p2);
    }

    public static byte @get_idasgn_header_by_short_name(void* p0, QString* p1, byte* p2)
    {
        if ((nint)_get_idasgn_header_by_short_name == 0) throw new System.EntryPointNotFoundException("'get_idasgn_header_by_short_name' is not available for the loaded IDA SDK version.");
        return _get_idasgn_header_by_short_name(p0, p1, p2);
    }

    public static byte @get_idasgn_path_by_short_name(QString* p0, byte* p1)
    {
        if ((nint)_get_idasgn_path_by_short_name == 0) throw new System.EntryPointNotFoundException("'get_idasgn_path_by_short_name' is not available for the loaded IDA SDK version.");
        return _get_idasgn_path_by_short_name(p0, p1);
    }

    public static int @get_idasgn_qty()
    {
        if ((nint)_get_idasgn_qty == 0) throw new System.EntryPointNotFoundException("'get_idasgn_qty' is not available for the loaded IDA SDK version.");
        return _get_idasgn_qty();
    }

    public static nint @get_idasgn_title(QString* p0, byte* p1)
    {
        if ((nint)_get_idasgn_title == 0) throw new System.EntryPointNotFoundException("'get_idasgn_title' is not available for the loaded IDA SDK version.");
        return _get_idasgn_title(p0, p1);
    }

    public static void* @get_idati()
    {
        if ((nint)_get_idati == 0) throw new System.EntryPointNotFoundException("'get_idati' is not available for the loaded IDA SDK version.");
        return _get_idati();
    }

    public static byte* @get_idc_filename(byte* p0, nuint p1, byte* p2)
    {
        if ((nint)_get_idc_filename == 0) throw new System.EntryPointNotFoundException("'get_idc_filename' is not available for the loaded IDA SDK version.");
        return _get_idc_filename(p0, p1, p2);
    }

    public static int @get_idcv_attr(void* p0, void* p1, byte* p2, byte p3)
    {
        if ((nint)_get_idcv_attr == 0) throw new System.EntryPointNotFoundException("'get_idcv_attr' is not available for the loaded IDA SDK version.");
        return _get_idcv_attr(p0, p1, p2, p3);
    }

    public static int @get_idcv_class_name(QString* p0, void* p1)
    {
        if ((nint)_get_idcv_class_name == 0) throw new System.EntryPointNotFoundException("'get_idcv_class_name' is not available for the loaded IDA SDK version.");
        return _get_idcv_class_name(p0, p1);
    }

    public static int @get_idcv_slice(void* p0, void* p1, ulong p2, ulong p3, int p4)
    {
        if ((nint)_get_idcv_slice == 0) throw new System.EntryPointNotFoundException("'get_idcv_slice' is not available for the loaded IDA SDK version.");
        return _get_idcv_slice(p0, p1, p2, p3, p4);
    }

    public static void* @get_idp_descs()
    {
        if ((nint)_get_idp_descs == 0) throw new System.EntryPointNotFoundException("'get_idp_descs' is not available for the loaded IDA SDK version.");
        return _get_idp_descs();
    }

    public static byte* @get_idp_name(byte* p0, nuint p1)
    {
        if ((nint)_get_idp_name == 0) throw new System.EntryPointNotFoundException("'get_idp_name' is not available for the loaded IDA SDK version.");
        return _get_idp_name(p0, p1);
    }

    public static nuint @get_immvals(ulong* p0, ulong p1, int p2, ulong p3, void* p4)
    {
        if ((nint)_get_immvals == 0) throw new System.EntryPointNotFoundException("'get_immvals' is not available for the loaded IDA SDK version.");
        return _get_immvals(p0, p1, p2, p3, p4);
    }

    public static byte @get_import_entry(void* p0, ulong p1)
    {
        if ((nint)_get_import_entry == 0) throw new System.EntryPointNotFoundException("'get_import_entry' is not available for the loaded IDA SDK version.");
        return _get_import_entry(p0, p1);
    }

    public static byte @get_import_module_name(QString* p0, int p1)
    {
        if ((nint)_get_import_module_name == 0) throw new System.EntryPointNotFoundException("'get_import_module_name' is not available for the loaded IDA SDK version.");
        return _get_import_module_name(p0, p1);
    }

    public static uint @get_import_module_qty()
    {
        if ((nint)_get_import_module_qty == 0) throw new System.EntryPointNotFoundException("'get_import_module_qty' is not available for the loaded IDA SDK version.");
        return _get_import_module_qty();
    }

    public static ulong @get_ind_purged(ulong p0)
    {
        if ((nint)_get_ind_purged == 0) throw new System.EntryPointNotFoundException("'get_ind_purged' is not available for the loaded IDA SDK version.");
        return _get_ind_purged(p0);
    }

    public static void @get_install_root(QString* p0)
    {
        if ((nint)_get_install_root == 0) throw new System.EntryPointNotFoundException("'get_install_root' is not available for the loaded IDA SDK version.");
        _get_install_root(p0);
    }

    public static uint @get_item_color(ulong p0)
    {
        if ((nint)_get_item_color == 0) throw new System.EntryPointNotFoundException("'get_item_color' is not available for the loaded IDA SDK version.");
        return _get_item_color(p0);
    }

    public static ulong @get_item_end(ulong p0)
    {
        if ((nint)_get_item_end == 0) throw new System.EntryPointNotFoundException("'get_item_end' is not available for the loaded IDA SDK version.");
        return _get_item_end(p0);
    }

    public static ulong @get_item_flag(ulong p0, int p1, ulong p2, byte p3)
    {
        if ((nint)_get_item_flag == 0) throw new System.EntryPointNotFoundException("'get_item_flag' is not available for the loaded IDA SDK version.");
        return _get_item_flag(p0, p1, p2, p3);
    }

    public static byte @get_item_refinfo(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_item_refinfo == 0) throw new System.EntryPointNotFoundException("'get_item_refinfo' is not available for the loaded IDA SDK version.");
        return _get_item_refinfo(p0, p1, p2);
    }

    public static ulong @get_jtable_target(ulong p0, void* p1, int p2)
    {
        if ((nint)_get_jtable_target == 0) throw new System.EntryPointNotFoundException("'get_jtable_target' is not available for the loaded IDA SDK version.");
        return _get_jtable_target(p0, p1, p2);
    }

    public static void* @get_last_hidden_range()
    {
        if ((nint)_get_last_hidden_range == 0) throw new System.EntryPointNotFoundException("'get_last_hidden_range' is not available for the loaded IDA SDK version.");
        return _get_last_hidden_range();
    }

    public static ulong @get_last_hidden_range_ea()
    {
        if ((nint)_get_last_hidden_range_ea == 0) throw new System.EntryPointNotFoundException("'get_last_hidden_range_ea' is not available for the loaded IDA SDK version.");
        return _get_last_hidden_range_ea();
    }

    public static int @get_last_pfxlen()
    {
        if ((nint)_get_last_pfxlen == 0) throw new System.EntryPointNotFoundException("'get_last_pfxlen' is not available for the loaded IDA SDK version.");
        return _get_last_pfxlen();
    }

    public static void* @get_last_seg()
    {
        if ((nint)_get_last_seg == 0) throw new System.EntryPointNotFoundException("'get_last_seg' is not available for the loaded IDA SDK version.");
        return _get_last_seg();
    }

    public static ulong @get_last_segment_ea()
    {
        if ((nint)_get_last_segment_ea == 0) throw new System.EntryPointNotFoundException("'get_last_segment_ea' is not available for the loaded IDA SDK version.");
        return _get_last_segment_ea();
    }

    public static byte @get_library_version(int* p0, int* p1, int* p2)
    {
        if ((nint)_get_library_version == 0) throw new System.EntryPointNotFoundException("'get_library_version' is not available for the loaded IDA SDK version.");
        return _get_library_version(p0, p1, p2);
    }

    public static nint @get_loader_name(byte* p0, nuint p1)
    {
        if ((nint)_get_loader_name == 0) throw new System.EntryPointNotFoundException("'get_loader_name' is not available for the loaded IDA SDK version.");
        return _get_loader_name(p0, p1);
    }

    public static byte* @get_loader_name_from_dll(byte* p0)
    {
        if ((nint)_get_loader_name_from_dll == 0) throw new System.EntryPointNotFoundException("'get_loader_name_from_dll' is not available for the loaded IDA SDK version.");
        return _get_loader_name_from_dll(p0);
    }

    public static int @get_logical_core_count()
    {
        if ((nint)_get_logical_core_count == 0) throw new System.EntryPointNotFoundException("'get_logical_core_count' is not available for the loaded IDA SDK version.");
        return _get_logical_core_count();
    }

    public static byte @get_login_name(QString* p0)
    {
        if ((nint)_get_login_name == 0) throw new System.EntryPointNotFoundException("'get_login_name' is not available for the loaded IDA SDK version.");
        return _get_login_name(p0);
    }

    public static int @get_lookback()
    {
        if ((nint)_get_lookback == 0) throw new System.EntryPointNotFoundException("'get_lookback' is not available for the loaded IDA SDK version.");
        return _get_lookback();
    }

    public static int @get_mangled_name_type(byte* p0)
    {
        if ((nint)_get_mangled_name_type == 0) throw new System.EntryPointNotFoundException("'get_mangled_name_type' is not available for the loaded IDA SDK version.");
        return _get_mangled_name_type(p0);
    }

    public static nint @get_manual_insn(QString* p0, ulong p1)
    {
        if ((nint)_get_manual_insn == 0) throw new System.EntryPointNotFoundException("'get_manual_insn' is not available for the loaded IDA SDK version.");
        return _get_manual_insn(p0, p1);
    }

    public static byte @get_mapping(ulong* p0, ulong* p1, ulong* p2, nuint p3)
    {
        if ((nint)_get_mapping == 0) throw new System.EntryPointNotFoundException("'get_mapping' is not available for the loaded IDA SDK version.");
        return _get_mapping(p0, p1, p2, p3);
    }

    public static nuint @get_mappings_qty()
    {
        if ((nint)_get_mappings_qty == 0) throw new System.EntryPointNotFoundException("'get_mappings_qty' is not available for the loaded IDA SDK version.");
        return _get_mappings_qty();
    }

    public static nuint @get_max_strlit_length(ulong p0, int p1, int p2)
    {
        if ((nint)_get_max_strlit_length == 0) throw new System.EntryPointNotFoundException("'get_max_strlit_length' is not available for the loaded IDA SDK version.");
        return _get_max_strlit_length(p0, p1, p2);
    }

    public static void* @get_module_data(int p0)
    {
        if ((nint)_get_module_data == 0) throw new System.EntryPointNotFoundException("'get_module_data' is not available for the loaded IDA SDK version.");
        return _get_module_data(p0);
    }

    public static ulong @get_name_base_ea(ulong p0, ulong p1)
    {
        if ((nint)_get_name_base_ea == 0) throw new System.EntryPointNotFoundException("'get_name_base_ea' is not available for the loaded IDA SDK version.");
        return _get_name_base_ea(p0, p1);
    }

    public static byte @get_name_color(ulong p0, ulong p1)
    {
        if ((nint)_get_name_color == 0) throw new System.EntryPointNotFoundException("'get_name_color' is not available for the loaded IDA SDK version.");
        return _get_name_color(p0, p1);
    }

    public static ulong @get_name_ea(ulong p0, byte* p1)
    {
        if ((nint)_get_name_ea == 0) throw new System.EntryPointNotFoundException("'get_name_ea' is not available for the loaded IDA SDK version.");
        return _get_name_ea(p0, p1);
    }

    public static nint @get_name_expr(QString* p0, ulong p1, int p2, ulong p3, ulong p4, int p5)
    {
        if ((nint)_get_name_expr == 0) throw new System.EntryPointNotFoundException("'get_name_expr' is not available for the loaded IDA SDK version.");
        return _get_name_expr(p0, p1, p2, p3, p4, p5);
    }

    public static int @get_name_value(ulong* p0, ulong p1, byte* p2)
    {
        if ((nint)_get_name_value == 0) throw new System.EntryPointNotFoundException("'get_name_value' is not available for the loaded IDA SDK version.");
        return _get_name_value(p0, p1, p2);
    }

    public static int @get_named_type(void* p0, byte* p1, int p2, byte** p3, byte** p4, byte** p5, byte** p6, int* p7, uint* p8)
    {
        if ((nint)_get_named_type == 0) throw new System.EntryPointNotFoundException("'get_named_type' is not available for the loaded IDA SDK version.");
        return _get_named_type(p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static ulong @get_named_type_tid(byte* p0)
    {
        if ((nint)_get_named_type_tid == 0) throw new System.EntryPointNotFoundException("'get_named_type_tid' is not available for the loaded IDA SDK version.");
        return _get_named_type_tid(p0);
    }

    public static ulong @get_next_cref_from(ulong p0, ulong p1)
    {
        if ((nint)_get_next_cref_from == 0) throw new System.EntryPointNotFoundException("'get_next_cref_from' is not available for the loaded IDA SDK version.");
        return _get_next_cref_from(p0, p1);
    }

    public static ulong @get_next_cref_to(ulong p0, ulong p1)
    {
        if ((nint)_get_next_cref_to == 0) throw new System.EntryPointNotFoundException("'get_next_cref_to' is not available for the loaded IDA SDK version.");
        return _get_next_cref_to(p0, p1);
    }

    public static ulong @get_next_dref_from(ulong p0, ulong p1)
    {
        if ((nint)_get_next_dref_from == 0) throw new System.EntryPointNotFoundException("'get_next_dref_from' is not available for the loaded IDA SDK version.");
        return _get_next_dref_from(p0, p1);
    }

    public static ulong @get_next_dref_to(ulong p0, ulong p1)
    {
        if ((nint)_get_next_dref_to == 0) throw new System.EntryPointNotFoundException("'get_next_dref_to' is not available for the loaded IDA SDK version.");
        return _get_next_dref_to(p0, p1);
    }

    public static void* @get_next_fchunk(ulong p0)
    {
        if ((nint)_get_next_fchunk == 0) throw new System.EntryPointNotFoundException("'get_next_fchunk' is not available for the loaded IDA SDK version.");
        return _get_next_fchunk(p0);
    }

    public static ulong @get_next_fchunk_ea(ulong p0)
    {
        if ((nint)_get_next_fchunk_ea == 0) throw new System.EntryPointNotFoundException("'get_next_fchunk_ea' is not available for the loaded IDA SDK version.");
        return _get_next_fchunk_ea(p0);
    }

    public static byte @get_next_fchunk_info(void* p0, ulong p1)
    {
        if ((nint)_get_next_fchunk_info == 0) throw new System.EntryPointNotFoundException("'get_next_fchunk_info' is not available for the loaded IDA SDK version.");
        return _get_next_fchunk_info(p0, p1);
    }

    public static ulong @get_next_fcref_from(ulong p0, ulong p1)
    {
        if ((nint)_get_next_fcref_from == 0) throw new System.EntryPointNotFoundException("'get_next_fcref_from' is not available for the loaded IDA SDK version.");
        return _get_next_fcref_from(p0, p1);
    }

    public static ulong @get_next_fcref_to(ulong p0, ulong p1)
    {
        if ((nint)_get_next_fcref_to == 0) throw new System.EntryPointNotFoundException("'get_next_fcref_to' is not available for the loaded IDA SDK version.");
        return _get_next_fcref_to(p0, p1);
    }

    public static ulong @get_next_fixup_ea(ulong p0)
    {
        if ((nint)_get_next_fixup_ea == 0) throw new System.EntryPointNotFoundException("'get_next_fixup_ea' is not available for the loaded IDA SDK version.");
        return _get_next_fixup_ea(p0);
    }

    public static void* @get_next_func(ulong p0)
    {
        if ((nint)_get_next_func == 0) throw new System.EntryPointNotFoundException("'get_next_func' is not available for the loaded IDA SDK version.");
        return _get_next_func(p0);
    }

    public static ulong @get_next_func_addr(void* p0, ulong p1)
    {
        if ((nint)_get_next_func_addr == 0) throw new System.EntryPointNotFoundException("'get_next_func_addr' is not available for the loaded IDA SDK version.");
        return _get_next_func_addr(p0, p1);
    }

    public static ulong @get_next_func_ea(ulong p0)
    {
        if ((nint)_get_next_func_ea == 0) throw new System.EntryPointNotFoundException("'get_next_func_ea' is not available for the loaded IDA SDK version.");
        return _get_next_func_ea(p0);
    }

    public static ulong @get_next_function_addr(ulong p0, ulong p1)
    {
        if ((nint)_get_next_function_addr == 0) throw new System.EntryPointNotFoundException("'get_next_function_addr' is not available for the loaded IDA SDK version.");
        return _get_next_function_addr(p0, p1);
    }

    public static void* @get_next_hidden_range(ulong p0)
    {
        if ((nint)_get_next_hidden_range == 0) throw new System.EntryPointNotFoundException("'get_next_hidden_range' is not available for the loaded IDA SDK version.");
        return _get_next_hidden_range(p0);
    }

    public static ulong @get_next_hidden_range_ea(ulong p0)
    {
        if ((nint)_get_next_hidden_range_ea == 0) throw new System.EntryPointNotFoundException("'get_next_hidden_range_ea' is not available for the loaded IDA SDK version.");
        return _get_next_hidden_range_ea(p0);
    }

    public static void* @get_next_seg(ulong p0)
    {
        if ((nint)_get_next_seg == 0) throw new System.EntryPointNotFoundException("'get_next_seg' is not available for the loaded IDA SDK version.");
        return _get_next_seg(p0);
    }

    public static ulong @get_next_segment_ea(ulong p0)
    {
        if ((nint)_get_next_segment_ea == 0) throw new System.EntryPointNotFoundException("'get_next_segment_ea' is not available for the loaded IDA SDK version.");
        return _get_next_segment_ea(p0);
    }

    public static nint @get_nice_colored_name(QString* p0, ulong p1, int p2)
    {
        if ((nint)_get_nice_colored_name == 0) throw new System.EntryPointNotFoundException("'get_nice_colored_name' is not available for the loaded IDA SDK version.");
        return _get_nice_colored_name(p0, p1, p2);
    }

    public static ulong @get_nlist_ea(nuint p0)
    {
        if ((nint)_get_nlist_ea == 0) throw new System.EntryPointNotFoundException("'get_nlist_ea' is not available for the loaded IDA SDK version.");
        return _get_nlist_ea(p0);
    }

    public static nuint @get_nlist_idx(ulong p0)
    {
        if ((nint)_get_nlist_idx == 0) throw new System.EntryPointNotFoundException("'get_nlist_idx' is not available for the loaded IDA SDK version.");
        return _get_nlist_idx(p0);
    }

    public static byte* @get_nlist_name(nuint p0)
    {
        if ((nint)_get_nlist_name == 0) throw new System.EntryPointNotFoundException("'get_nlist_name' is not available for the loaded IDA SDK version.");
        return _get_nlist_name(p0);
    }

    public static nuint @get_nlist_size()
    {
        if ((nint)_get_nlist_size == 0) throw new System.EntryPointNotFoundException("'get_nlist_size' is not available for the loaded IDA SDK version.");
        return _get_nlist_size();
    }

    public static byte @get_node_info(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_node_info == 0) throw new System.EntryPointNotFoundException("'get_node_info' is not available for the loaded IDA SDK version.");
        return _get_node_info(p0, p1, p2);
    }

    public static ulong @get_nsec_stamp()
    {
        if ((nint)_get_nsec_stamp == 0) throw new System.EntryPointNotFoundException("'get_nsec_stamp' is not available for the loaded IDA SDK version.");
        return _get_nsec_stamp();
    }

    public static byte @get_numbered_type(void* p0, uint p1, byte** p2, byte** p3, byte** p4, byte** p5, int* p6)
    {
        if ((nint)_get_numbered_type == 0) throw new System.EntryPointNotFoundException("'get_numbered_type' is not available for the loaded IDA SDK version.");
        return _get_numbered_type(p0, p1, p2, p3, p4, p5, p6);
    }

    public static byte* @get_numbered_type_name(void* p0, uint p1)
    {
        if ((nint)_get_numbered_type_name == 0) throw new System.EntryPointNotFoundException("'get_numbered_type_name' is not available for the loaded IDA SDK version.");
        return _get_numbered_type_name(p0, p1);
    }

    public static byte @get_octet(byte* p0, void* p1)
    {
        if ((nint)_get_octet == 0) throw new System.EntryPointNotFoundException("'get_octet' is not available for the loaded IDA SDK version.");
        return _get_octet(p0, p1);
    }

    public static int @get_offset_expr(QString* p0, ulong p1, int p2, void* p3, ulong p4, long p5, int p6)
    {
        if ((nint)_get_offset_expr == 0) throw new System.EntryPointNotFoundException("'get_offset_expr' is not available for the loaded IDA SDK version.");
        return _get_offset_expr(p0, p1, p2, p3, p4, p5, p6);
    }

    public static int @get_offset_expression(QString* p0, ulong p1, int p2, ulong p3, long p4, int p5)
    {
        if ((nint)_get_offset_expression == 0) throw new System.EntryPointNotFoundException("'get_offset_expression' is not available for the loaded IDA SDK version.");
        return _get_offset_expression(p0, p1, p2, p3, p4, p5);
    }

    public static byte @get_op_tinfo(TypeInfo* p0, ulong p1, int p2)
    {
        if ((nint)_get_op_tinfo == 0) throw new System.EntryPointNotFoundException("'get_op_tinfo' is not available for the loaded IDA SDK version.");
        return _get_op_tinfo(p0, p1, p2);
    }

    public static void* @get_opinfo(void* p0, ulong p1, int p2, ulong p3)
    {
        if ((nint)_get_opinfo == 0) throw new System.EntryPointNotFoundException("'get_opinfo' is not available for the loaded IDA SDK version.");
        return _get_opinfo(p0, p1, p2, p3);
    }

    public static uint @get_ordinal_limit(void* p0)
    {
        if ((nint)_get_ordinal_limit == 0) throw new System.EntryPointNotFoundException("'get_ordinal_limit' is not available for the loaded IDA SDK version.");
        return _get_ordinal_limit(p0);
    }

    public static ulong @get_original_byte(ulong p0)
    {
        if ((nint)_get_original_byte == 0) throw new System.EntryPointNotFoundException("'get_original_byte' is not available for the loaded IDA SDK version.");
        return _get_original_byte(p0);
    }

    public static ulong @get_original_dword(ulong p0)
    {
        if ((nint)_get_original_dword == 0) throw new System.EntryPointNotFoundException("'get_original_dword' is not available for the loaded IDA SDK version.");
        return _get_original_dword(p0);
    }

    public static ulong @get_original_qword(ulong p0)
    {
        if ((nint)_get_original_qword == 0) throw new System.EntryPointNotFoundException("'get_original_qword' is not available for the loaded IDA SDK version.");
        return _get_original_qword(p0);
    }

    public static ulong @get_original_word(ulong p0)
    {
        if ((nint)_get_original_word == 0) throw new System.EntryPointNotFoundException("'get_original_word' is not available for the loaded IDA SDK version.");
        return _get_original_word(p0);
    }

    public static int @get_outfile_encoding_idx()
    {
        if ((nint)_get_outfile_encoding_idx == 0) throw new System.EntryPointNotFoundException("'get_outfile_encoding_idx' is not available for the loaded IDA SDK version.");
        return _get_outfile_encoding_idx();
    }

    public static byte @get_parser_option(QString* p0, byte* p1, byte* p2)
    {
        if ((nint)_get_parser_option == 0) throw new System.EntryPointNotFoundException("'get_parser_option' is not available for the loaded IDA SDK version.");
        return _get_parser_option(p0, p1, p2);
    }

    public static byte* @get_path(int p0)
    {
        if ((nint)_get_path == 0) throw new System.EntryPointNotFoundException("'get_path' is not available for the loaded IDA SDK version.");
        return _get_path(p0);
    }

    public static void* @get_ph()
    {
        if ((nint)_get_ph == 0) throw new System.EntryPointNotFoundException("'get_ph' is not available for the loaded IDA SDK version.");
        return _get_ph();
    }

    public static int @get_physical_core_count()
    {
        if ((nint)_get_physical_core_count == 0) throw new System.EntryPointNotFoundException("'get_physical_core_count' is not available for the loaded IDA SDK version.");
        return _get_physical_core_count();
    }

    public static void* @get_place_class(int* p0, int* p1, int p2)
    {
        if ((nint)_get_place_class == 0) throw new System.EntryPointNotFoundException("'get_place_class' is not available for the loaded IDA SDK version.");
        return _get_place_class(p0, p1, p2);
    }

    public static int @get_place_class_id(byte* p0)
    {
        if ((nint)_get_place_class_id == 0) throw new System.EntryPointNotFoundException("'get_place_class_id' is not available for the loaded IDA SDK version.");
        return _get_place_class_id(p0);
    }

    public static byte* @get_plugin_options(byte* p0)
    {
        if ((nint)_get_plugin_options == 0) throw new System.EntryPointNotFoundException("'get_plugin_options' is not available for the loaded IDA SDK version.");
        return _get_plugin_options(p0);
    }

    public static void* @get_plugins()
    {
        if ((nint)_get_plugins == 0) throw new System.EntryPointNotFoundException("'get_plugins' is not available for the loaded IDA SDK version.");
        return _get_plugins();
    }

    public static ulong @get_possible_item_varsize(ulong p0, TypeInfo* p1)
    {
        if ((nint)_get_possible_item_varsize == 0) throw new System.EntryPointNotFoundException("'get_possible_item_varsize' is not available for the loaded IDA SDK version.");
        return _get_possible_item_varsize(p0, p1);
    }

    public static nint @get_predef_insn_cmt(QString* p0, void* p1)
    {
        if ((nint)_get_predef_insn_cmt == 0) throw new System.EntryPointNotFoundException("'get_predef_insn_cmt' is not available for the loaded IDA SDK version.");
        return _get_predef_insn_cmt(p0, p1);
    }

    public static void* @get_prev_fchunk(ulong p0)
    {
        if ((nint)_get_prev_fchunk == 0) throw new System.EntryPointNotFoundException("'get_prev_fchunk' is not available for the loaded IDA SDK version.");
        return _get_prev_fchunk(p0);
    }

    public static ulong @get_prev_fchunk_ea(ulong p0)
    {
        if ((nint)_get_prev_fchunk_ea == 0) throw new System.EntryPointNotFoundException("'get_prev_fchunk_ea' is not available for the loaded IDA SDK version.");
        return _get_prev_fchunk_ea(p0);
    }

    public static byte @get_prev_fchunk_info(void* p0, ulong p1)
    {
        if ((nint)_get_prev_fchunk_info == 0) throw new System.EntryPointNotFoundException("'get_prev_fchunk_info' is not available for the loaded IDA SDK version.");
        return _get_prev_fchunk_info(p0, p1);
    }

    public static ulong @get_prev_fixup_ea(ulong p0)
    {
        if ((nint)_get_prev_fixup_ea == 0) throw new System.EntryPointNotFoundException("'get_prev_fixup_ea' is not available for the loaded IDA SDK version.");
        return _get_prev_fixup_ea(p0);
    }

    public static void* @get_prev_func(ulong p0)
    {
        if ((nint)_get_prev_func == 0) throw new System.EntryPointNotFoundException("'get_prev_func' is not available for the loaded IDA SDK version.");
        return _get_prev_func(p0);
    }

    public static ulong @get_prev_func_addr(void* p0, ulong p1)
    {
        if ((nint)_get_prev_func_addr == 0) throw new System.EntryPointNotFoundException("'get_prev_func_addr' is not available for the loaded IDA SDK version.");
        return _get_prev_func_addr(p0, p1);
    }

    public static ulong @get_prev_func_ea(ulong p0)
    {
        if ((nint)_get_prev_func_ea == 0) throw new System.EntryPointNotFoundException("'get_prev_func_ea' is not available for the loaded IDA SDK version.");
        return _get_prev_func_ea(p0);
    }

    public static ulong @get_prev_function_addr(ulong p0, ulong p1)
    {
        if ((nint)_get_prev_function_addr == 0) throw new System.EntryPointNotFoundException("'get_prev_function_addr' is not available for the loaded IDA SDK version.");
        return _get_prev_function_addr(p0, p1);
    }

    public static void* @get_prev_hidden_range(ulong p0)
    {
        if ((nint)_get_prev_hidden_range == 0) throw new System.EntryPointNotFoundException("'get_prev_hidden_range' is not available for the loaded IDA SDK version.");
        return _get_prev_hidden_range(p0);
    }

    public static ulong @get_prev_hidden_range_ea(ulong p0)
    {
        if ((nint)_get_prev_hidden_range_ea == 0) throw new System.EntryPointNotFoundException("'get_prev_hidden_range_ea' is not available for the loaded IDA SDK version.");
        return _get_prev_hidden_range_ea(p0);
    }

    public static void* @get_prev_seg(ulong p0)
    {
        if ((nint)_get_prev_seg == 0) throw new System.EntryPointNotFoundException("'get_prev_seg' is not available for the loaded IDA SDK version.");
        return _get_prev_seg(p0);
    }

    public static ulong @get_prev_segment_ea(ulong p0)
    {
        if ((nint)_get_prev_segment_ea == 0) throw new System.EntryPointNotFoundException("'get_prev_segment_ea' is not available for the loaded IDA SDK version.");
        return _get_prev_segment_ea(p0);
    }

    public static byte @get_prev_sreg_range(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_prev_sreg_range == 0) throw new System.EntryPointNotFoundException("'get_prev_sreg_range' is not available for the loaded IDA SDK version.");
        return _get_prev_sreg_range(p0, p1, p2);
    }

    public static ulong @get_problem(byte p0, ulong p1)
    {
        if ((nint)_get_problem == 0) throw new System.EntryPointNotFoundException("'get_problem' is not available for the loaded IDA SDK version.");
        return _get_problem(p0, p1);
    }

    public static nint @get_problem_desc(QString* p0, byte p1, ulong p2)
    {
        if ((nint)_get_problem_desc == 0) throw new System.EntryPointNotFoundException("'get_problem_desc' is not available for the loaded IDA SDK version.");
        return _get_problem_desc(p0, p1, p2);
    }

    public static byte* @get_problem_name(byte p0, byte p1)
    {
        if ((nint)_get_problem_name == 0) throw new System.EntryPointNotFoundException("'get_problem_name' is not available for the loaded IDA SDK version.");
        return _get_problem_name(p0, p1);
    }

    public static int @get_qerrno()
    {
        if ((nint)_get_qerrno == 0) throw new System.EntryPointNotFoundException("'get_qerrno' is not available for the loaded IDA SDK version.");
        return _get_qerrno();
    }

    public static ulong @get_qword(ulong p0)
    {
        if ((nint)_get_qword == 0) throw new System.EntryPointNotFoundException("'get_qword' is not available for the loaded IDA SDK version.");
        return _get_qword(p0);
    }

    public static int @get_radix(ulong p0, int p1)
    {
        if ((nint)_get_radix == 0) throw new System.EntryPointNotFoundException("'get_radix' is not available for the loaded IDA SDK version.");
        return _get_radix(p0, p1);
    }

    public static byte @get_realtype(void* p0, byte* p1)
    {
        if ((nint)_get_realtype == 0) throw new System.EntryPointNotFoundException("'get_realtype' is not available for the loaded IDA SDK version.");
        return _get_realtype(p0, p1);
    }

    public static byte @get_redo_action_label(QString* p0)
    {
        if ((nint)_get_redo_action_label == 0) throw new System.EntryPointNotFoundException("'get_redo_action_label' is not available for the loaded IDA SDK version.");
        return _get_redo_action_label(p0);
    }

    public static byte @get_refinfo(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_refinfo == 0) throw new System.EntryPointNotFoundException("'get_refinfo' is not available for the loaded IDA SDK version.");
        return _get_refinfo(p0, p1, p2);
    }

    public static void @get_refinfo_descs(void* p0)
    {
        if ((nint)_get_refinfo_descs == 0) throw new System.EntryPointNotFoundException("'get_refinfo_descs' is not available for the loaded IDA SDK version.");
        _get_refinfo_descs(p0);
    }

    public static byte @get_reftype_by_size(nuint p0)
    {
        if ((nint)_get_reftype_by_size == 0) throw new System.EntryPointNotFoundException("'get_reftype_by_size' is not available for the loaded IDA SDK version.");
        return _get_reftype_by_size(p0);
    }

    public static nint @get_reg_name(QString* p0, int p1, nuint p2, int p3)
    {
        if ((nint)_get_reg_name == 0) throw new System.EntryPointNotFoundException("'get_reg_name' is not available for the loaded IDA SDK version.");
        return _get_reg_name(p0, p1, p2, p3);
    }

    public static nint @get_root_filename(byte* p0, nuint p1)
    {
        if ((nint)_get_root_filename == 0) throw new System.EntryPointNotFoundException("'get_root_filename' is not available for the loaded IDA SDK version.");
        return _get_root_filename(p0, p1);
    }

    public static ulong @get_segm_base(void* p0)
    {
        if ((nint)_get_segm_base == 0) throw new System.EntryPointNotFoundException("'get_segm_base' is not available for the loaded IDA SDK version.");
        return _get_segm_base(p0);
    }

    public static void* @get_segm_by_name(byte* p0)
    {
        if ((nint)_get_segm_by_name == 0) throw new System.EntryPointNotFoundException("'get_segm_by_name' is not available for the loaded IDA SDK version.");
        return _get_segm_by_name(p0);
    }

    public static void* @get_segm_by_sel(ulong p0)
    {
        if ((nint)_get_segm_by_sel == 0) throw new System.EntryPointNotFoundException("'get_segm_by_sel' is not available for the loaded IDA SDK version.");
        return _get_segm_by_sel(p0);
    }

    public static nint @get_segm_class(QString* p0, void* p1)
    {
        if ((nint)_get_segm_class == 0) throw new System.EntryPointNotFoundException("'get_segm_class' is not available for the loaded IDA SDK version.");
        return _get_segm_class(p0, p1);
    }

    public static nint @get_segm_name(QString* p0, void* p1, int p2)
    {
        if ((nint)_get_segm_name == 0) throw new System.EntryPointNotFoundException("'get_segm_name' is not available for the loaded IDA SDK version.");
        return _get_segm_name(p0, p1, p2);
    }

    public static int @get_segm_num(ulong p0)
    {
        if ((nint)_get_segm_num == 0) throw new System.EntryPointNotFoundException("'get_segm_num' is not available for the loaded IDA SDK version.");
        return _get_segm_num(p0);
    }

    public static ulong @get_segm_para(void* p0)
    {
        if ((nint)_get_segm_para == 0) throw new System.EntryPointNotFoundException("'get_segm_para' is not available for the loaded IDA SDK version.");
        return _get_segm_para(p0);
    }

    public static int @get_segm_qty()
    {
        if ((nint)_get_segm_qty == 0) throw new System.EntryPointNotFoundException("'get_segm_qty' is not available for the loaded IDA SDK version.");
        return _get_segm_qty();
    }

    public static byte* @get_segment_alignment(byte p0)
    {
        if ((nint)_get_segment_alignment == 0) throw new System.EntryPointNotFoundException("'get_segment_alignment' is not available for the loaded IDA SDK version.");
        return _get_segment_alignment(p0);
    }

    public static ulong @get_segment_base(ulong p0)
    {
        if ((nint)_get_segment_base == 0) throw new System.EntryPointNotFoundException("'get_segment_base' is not available for the loaded IDA SDK version.");
        return _get_segment_base(p0);
    }

    public static nint @get_segment_class(QString* p0, ulong p1)
    {
        if ((nint)_get_segment_class == 0) throw new System.EntryPointNotFoundException("'get_segment_class' is not available for the loaded IDA SDK version.");
        return _get_segment_class(p0, p1);
    }

    public static nint @get_segment_cmt(QString* p0, void* p1, byte p2)
    {
        if ((nint)_get_segment_cmt == 0) throw new System.EntryPointNotFoundException("'get_segment_cmt' is not available for the loaded IDA SDK version.");
        return _get_segment_cmt(p0, p1, p2);
    }

    public static nint @get_segment_cmt_by_ea(QString* p0, ulong p1, byte p2)
    {
        if ((nint)_get_segment_cmt_by_ea == 0) throw new System.EntryPointNotFoundException("'get_segment_cmt_by_ea' is not available for the loaded IDA SDK version.");
        return _get_segment_cmt_by_ea(p0, p1, p2);
    }

    public static byte* @get_segment_combination(byte p0)
    {
        if ((nint)_get_segment_combination == 0) throw new System.EntryPointNotFoundException("'get_segment_combination' is not available for the loaded IDA SDK version.");
        return _get_segment_combination(p0);
    }

    public static ulong @get_segment_ea(ulong p0)
    {
        if ((nint)_get_segment_ea == 0) throw new System.EntryPointNotFoundException("'get_segment_ea' is not available for the loaded IDA SDK version.");
        return _get_segment_ea(p0);
    }

    public static ulong @get_segment_ea_by_name(byte* p0)
    {
        if ((nint)_get_segment_ea_by_name == 0) throw new System.EntryPointNotFoundException("'get_segment_ea_by_name' is not available for the loaded IDA SDK version.");
        return _get_segment_ea_by_name(p0);
    }

    public static ulong @get_segment_ea_by_num(int p0)
    {
        if ((nint)_get_segment_ea_by_num == 0) throw new System.EntryPointNotFoundException("'get_segment_ea_by_num' is not available for the loaded IDA SDK version.");
        return _get_segment_ea_by_num(p0);
    }

    public static ulong @get_segment_ea_by_sel(ulong p0)
    {
        if ((nint)_get_segment_ea_by_sel == 0) throw new System.EntryPointNotFoundException("'get_segment_ea_by_sel' is not available for the loaded IDA SDK version.");
        return _get_segment_ea_by_sel(p0);
    }

    public static byte @get_segment_info(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_segment_info == 0) throw new System.EntryPointNotFoundException("'get_segment_info' is not available for the loaded IDA SDK version.");
        return _get_segment_info(p0, p1, p2);
    }

    public static byte @get_segment_info_by_num(void* p0, int p1, int p2)
    {
        if ((nint)_get_segment_info_by_num == 0) throw new System.EntryPointNotFoundException("'get_segment_info_by_num' is not available for the loaded IDA SDK version.");
        return _get_segment_info_by_num(p0, p1, p2);
    }

    public static nint @get_segment_name(QString* p0, ulong p1, int p2)
    {
        if ((nint)_get_segment_name == 0) throw new System.EntryPointNotFoundException("'get_segment_name' is not available for the loaded IDA SDK version.");
        return _get_segment_name(p0, p1, p2);
    }

    public static ulong @get_segment_para(ulong p0)
    {
        if ((nint)_get_segment_para == 0) throw new System.EntryPointNotFoundException("'get_segment_para' is not available for the loaded IDA SDK version.");
        return _get_segment_para(p0);
    }

    public static nint @get_segment_translations(void* p0, ulong p1)
    {
        if ((nint)_get_segment_translations == 0) throw new System.EntryPointNotFoundException("'get_segment_translations' is not available for the loaded IDA SDK version.");
        return _get_segment_translations(p0, p1);
    }

    public static byte @get_selected_parser_name(QString* p0)
    {
        if ((nint)_get_selected_parser_name == 0) throw new System.EntryPointNotFoundException("'get_selected_parser_name' is not available for the loaded IDA SDK version.");
        return _get_selected_parser_name(p0);
    }

    public static nuint @get_selector_qty()
    {
        if ((nint)_get_selector_qty == 0) throw new System.EntryPointNotFoundException("'get_selector_qty' is not available for the loaded IDA SDK version.");
        return _get_selector_qty();
    }

    public static void* @get_server_connection()
    {
        if ((nint)_get_server_connection == 0) throw new System.EntryPointNotFoundException("'get_server_connection' is not available for the loaded IDA SDK version.");
        return _get_server_connection();
    }

    public static void* @get_server_connection2(int p0)
    {
        if ((nint)_get_server_connection2 == 0) throw new System.EntryPointNotFoundException("'get_server_connection2' is not available for the loaded IDA SDK version.");
        return _get_server_connection2(p0);
    }

    public static ulong @get_source_linnum(ulong p0)
    {
        if ((nint)_get_source_linnum == 0) throw new System.EntryPointNotFoundException("'get_source_linnum' is not available for the loaded IDA SDK version.");
        return _get_source_linnum(p0);
    }

    public static byte* @get_sourcefile(ulong p0, void* p1)
    {
        if ((nint)_get_sourcefile == 0) throw new System.EntryPointNotFoundException("'get_sourcefile' is not available for the loaded IDA SDK version.");
        return _get_sourcefile(p0, p1);
    }

    public static byte @get_sourcefile_by_ea(QString* p0, ulong p1, void* p2)
    {
        if ((nint)_get_sourcefile_by_ea == 0) throw new System.EntryPointNotFoundException("'get_sourcefile_by_ea' is not available for the loaded IDA SDK version.");
        return _get_sourcefile_by_ea(p0, p1, p2);
    }

    public static nuint @get_sourcefiles_qty()
    {
        if ((nint)_get_sourcefiles_qty == 0) throw new System.EntryPointNotFoundException("'get_sourcefiles_qty' is not available for the loaded IDA SDK version.");
        return _get_sourcefiles_qty();
    }

    public static long @get_sp_delta(void* p0, ulong p1)
    {
        if ((nint)_get_sp_delta == 0) throw new System.EntryPointNotFoundException("'get_sp_delta' is not available for the loaded IDA SDK version.");
        return _get_sp_delta(p0, p1);
    }

    public static long @get_spd(void* p0, ulong p1)
    {
        if ((nint)_get_spd == 0) throw new System.EntryPointNotFoundException("'get_spd' is not available for the loaded IDA SDK version.");
        return _get_spd(p0, p1);
    }

    public static byte @get_special_folder(byte* p0, nuint p1, int p2)
    {
        if ((nint)_get_special_folder == 0) throw new System.EntryPointNotFoundException("'get_special_folder' is not available for the loaded IDA SDK version.");
        return _get_special_folder(p0, p1, p2);
    }

    public static int @get_spoiled_reg(void* p0, uint* p1, nuint p2)
    {
        if ((nint)_get_spoiled_reg == 0) throw new System.EntryPointNotFoundException("'get_spoiled_reg' is not available for the loaded IDA SDK version.");
        return _get_spoiled_reg(p0, p1, p2);
    }

    public static ulong @get_sreg(ulong p0, int p1)
    {
        if ((nint)_get_sreg == 0) throw new System.EntryPointNotFoundException("'get_sreg' is not available for the loaded IDA SDK version.");
        return _get_sreg(p0, p1);
    }

    public static byte @get_sreg_range(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_sreg_range == 0) throw new System.EntryPointNotFoundException("'get_sreg_range' is not available for the loaded IDA SDK version.");
        return _get_sreg_range(p0, p1, p2);
    }

    public static int @get_sreg_range_num(ulong p0, int p1)
    {
        if ((nint)_get_sreg_range_num == 0) throw new System.EntryPointNotFoundException("'get_sreg_range_num' is not available for the loaded IDA SDK version.");
        return _get_sreg_range_num(p0, p1);
    }

    public static nuint @get_sreg_ranges_qty(int p0)
    {
        if ((nint)_get_sreg_ranges_qty == 0) throw new System.EntryPointNotFoundException("'get_sreg_ranges_qty' is not available for the loaded IDA SDK version.");
        return _get_sreg_ranges_qty(p0);
    }

    public static void* @get_std_dirtree(int p0)
    {
        if ((nint)_get_std_dirtree == 0) throw new System.EntryPointNotFoundException("'get_std_dirtree' is not available for the loaded IDA SDK version.");
        return _get_std_dirtree(p0);
    }

    public static byte @get_stkarg_area_info(void* p0, uint p1)
    {
        if ((nint)_get_stkarg_area_info == 0) throw new System.EntryPointNotFoundException("'get_stkarg_area_info' is not available for the loaded IDA SDK version.");
        return _get_stkarg_area_info(p0, p1);
    }

    public static byte @get_stock_tinfo(TypeInfo* p0, int p1)
    {
        if ((nint)_get_stock_tinfo == 0) throw new System.EntryPointNotFoundException("'get_stock_tinfo' is not available for the loaded IDA SDK version.");
        return _get_stock_tinfo(p0, p1);
    }

    public static uint @get_str_type(ulong p0)
    {
        if ((nint)_get_str_type == 0) throw new System.EntryPointNotFoundException("'get_str_type' is not available for the loaded IDA SDK version.");
        return _get_str_type(p0);
    }

    public static ulong @get_strid(ulong p0)
    {
        if ((nint)_get_strid == 0) throw new System.EntryPointNotFoundException("'get_strid' is not available for the loaded IDA SDK version.");
        return _get_strid(p0);
    }

    public static byte @get_strlist_item(void* p0, nuint p1)
    {
        if ((nint)_get_strlist_item == 0) throw new System.EntryPointNotFoundException("'get_strlist_item' is not available for the loaded IDA SDK version.");
        return _get_strlist_item(p0, p1);
    }

    public static byte @get_strlist_item_ex(void* p0, nuint p1)
    {
        if ((nint)_get_strlist_item_ex == 0) throw new System.EntryPointNotFoundException("'get_strlist_item_ex' is not available for the loaded IDA SDK version.");
        return _get_strlist_item_ex(p0, p1);
    }

    public static void* @get_strlist_options()
    {
        if ((nint)_get_strlist_options == 0) throw new System.EntryPointNotFoundException("'get_strlist_options' is not available for the loaded IDA SDK version.");
        return _get_strlist_options();
    }

    public static nuint @get_strlist_qty()
    {
        if ((nint)_get_strlist_qty == 0) throw new System.EntryPointNotFoundException("'get_strlist_qty' is not available for the loaded IDA SDK version.");
        return _get_strlist_qty();
    }

    public static nint @get_strlit_contents(QString* p0, ulong p1, nuint p2, int p3, nuint* p4, int p5)
    {
        if ((nint)_get_strlit_contents == 0) throw new System.EntryPointNotFoundException("'get_strlit_contents' is not available for the loaded IDA SDK version.");
        return _get_strlit_contents(p0, p1, p2, p3, p4, p5);
    }

    public static int @get_stroff_path(ulong* p0, long* p1, ulong p2, int p3)
    {
        if ((nint)_get_stroff_path == 0) throw new System.EntryPointNotFoundException("'get_stroff_path' is not available for the loaded IDA SDK version.");
        return _get_stroff_path(p0, p1, p2, p3);
    }

    public static int @get_struct_operand(long* p0, long* p1, ulong* p2, ulong p3, int p4)
    {
        if ((nint)_get_struct_operand == 0) throw new System.EntryPointNotFoundException("'get_struct_operand' is not available for the loaded IDA SDK version.");
        return _get_struct_operand(p0, p1, p2, p3, p4);
    }

    public static nint @get_switch_info(void* p0, ulong p1)
    {
        if ((nint)_get_switch_info == 0) throw new System.EntryPointNotFoundException("'get_switch_info' is not available for the loaded IDA SDK version.");
        return _get_switch_info(p0, p1);
    }

    public static ulong @get_tail_owner(ulong p0)
    {
        if ((nint)_get_tail_owner == 0) throw new System.EntryPointNotFoundException("'get_tail_owner' is not available for the loaded IDA SDK version.");
        return _get_tail_owner(p0);
    }

    public static ulong @get_tail_referer(ulong p0, nuint p1)
    {
        if ((nint)_get_tail_referer == 0) throw new System.EntryPointNotFoundException("'get_tail_referer' is not available for the loaded IDA SDK version.");
        return _get_tail_referer(p0, p1);
    }

    public static nuint @get_tail_referer_qty(ulong p0)
    {
        if ((nint)_get_tail_referer_qty == 0) throw new System.EntryPointNotFoundException("'get_tail_referer_qty' is not available for the loaded IDA SDK version.");
        return _get_tail_referer_qty(p0);
    }

    public static byte @get_tail_referers(void* p0, ulong p1)
    {
        if ((nint)_get_tail_referers == 0) throw new System.EntryPointNotFoundException("'get_tail_referers' is not available for the loaded IDA SDK version.");
        return _get_tail_referers(p0, p1);
    }

    public static byte @get_tid_name(QString* p0, ulong p1)
    {
        if ((nint)_get_tid_name == 0) throw new System.EntryPointNotFoundException("'get_tid_name' is not available for the loaded IDA SDK version.");
        return _get_tid_name(p0, p1);
    }

    public static uint @get_tid_ordinal(ulong p0)
    {
        if ((nint)_get_tid_ordinal == 0) throw new System.EntryPointNotFoundException("'get_tid_ordinal' is not available for the loaded IDA SDK version.");
        return _get_tid_ordinal(p0);
    }

    public static byte @get_tinfo(TypeInfo* p0, ulong p1)
    {
        if ((nint)_get_tinfo == 0) throw new System.EntryPointNotFoundException("'get_tinfo' is not available for the loaded IDA SDK version.");
        return _get_tinfo(p0, p1);
    }

    public static byte @get_tinfo_attr(ulong p0, QString* p1, void* p2, byte p3)
    {
        if ((nint)_get_tinfo_attr == 0) throw new System.EntryPointNotFoundException("'get_tinfo_attr' is not available for the loaded IDA SDK version.");
        return _get_tinfo_attr(p0, p1, p2, p3);
    }

    public static byte @get_tinfo_attrs(ulong p0, void* p1, byte p2)
    {
        if ((nint)_get_tinfo_attrs == 0) throw new System.EntryPointNotFoundException("'get_tinfo_attrs' is not available for the loaded IDA SDK version.");
        return _get_tinfo_attrs(p0, p1, p2);
    }

    public static nint @get_tinfo_by_edm_name(TypeInfo* p0, void* p1, byte* p2)
    {
        if ((nint)_get_tinfo_by_edm_name == 0) throw new System.EntryPointNotFoundException("'get_tinfo_by_edm_name' is not available for the loaded IDA SDK version.");
        return _get_tinfo_by_edm_name(p0, p1, p2);
    }

    public static byte @get_tinfo_by_flags(TypeInfo* p0, ulong p1)
    {
        if ((nint)_get_tinfo_by_flags == 0) throw new System.EntryPointNotFoundException("'get_tinfo_by_flags' is not available for the loaded IDA SDK version.");
        return _get_tinfo_by_flags(p0, p1);
    }

    public static byte @get_tinfo_details(ulong p0, byte p1, void* p2)
    {
        if ((nint)_get_tinfo_details == 0) throw new System.EntryPointNotFoundException("'get_tinfo_details' is not available for the loaded IDA SDK version.");
        return _get_tinfo_details(p0, p1, p2);
    }

    public static nuint @get_tinfo_pdata(void* p0, ulong p1, int p2)
    {
        if ((nint)_get_tinfo_pdata == 0) throw new System.EntryPointNotFoundException("'get_tinfo_pdata' is not available for the loaded IDA SDK version.");
        return _get_tinfo_pdata(p0, p1, p2);
    }

    public static nuint @get_tinfo_property(ulong p0, int p1)
    {
        if ((nint)_get_tinfo_property == 0) throw new System.EntryPointNotFoundException("'get_tinfo_property' is not available for the loaded IDA SDK version.");
        return _get_tinfo_property(p0, p1);
    }

    public static nuint @get_tinfo_property4(ulong p0, int p1, nuint p2, nuint p3, nuint p4, nuint p5)
    {
        if ((nint)_get_tinfo_property4 == 0) throw new System.EntryPointNotFoundException("'get_tinfo_property4' is not available for the loaded IDA SDK version.");
        return _get_tinfo_property4(p0, p1, p2, p3, p4, p5);
    }

    public static nuint @get_tinfo_size(uint* p0, ulong p1, int p2)
    {
        if ((nint)_get_tinfo_size == 0) throw new System.EntryPointNotFoundException("'get_tinfo_size' is not available for the loaded IDA SDK version.");
        return _get_tinfo_size(p0, p1, p2);
    }

    public static ulong @get_tinfo_tid(TypeInfo* p0, byte p1)
    {
        if ((nint)_get_tinfo_tid == 0) throw new System.EntryPointNotFoundException("'get_tinfo_tid' is not available for the loaded IDA SDK version.");
        return _get_tinfo_tid(p0, p1);
    }

    public static nuint @get_tryblks(void* p0, void* p1)
    {
        if ((nint)_get_tryblks == 0) throw new System.EntryPointNotFoundException("'get_tryblks' is not available for the loaded IDA SDK version.");
        return _get_tryblks(p0, p1);
    }

    public static byte @get_type_by_tid(TypeInfo* p0, ulong p1)
    {
        if ((nint)_get_type_by_tid == 0) throw new System.EntryPointNotFoundException("'get_type_by_tid' is not available for the loaded IDA SDK version.");
        return _get_type_by_tid(p0, p1);
    }

    public static int @get_type_ordinal(void* p0, byte* p1)
    {
        if ((nint)_get_type_ordinal == 0) throw new System.EntryPointNotFoundException("'get_type_ordinal' is not available for the loaded IDA SDK version.");
        return _get_type_ordinal(p0, p1);
    }

    public static nint @get_udm_by_fullname(void* p0, byte* p1)
    {
        if ((nint)_get_udm_by_fullname == 0) throw new System.EntryPointNotFoundException("'get_udm_by_fullname' is not available for the loaded IDA SDK version.");
        return _get_udm_by_fullname(p0, p1);
    }

    public static nint @get_udm_by_tid(TypeInfo* p0, void* p1, ulong p2)
    {
        if ((nint)_get_udm_by_tid == 0) throw new System.EntryPointNotFoundException("'get_udm_by_tid' is not available for the loaded IDA SDK version.");
        return _get_udm_by_tid(p0, p1, p2);
    }

    public static byte @get_undo_action_label(QString* p0)
    {
        if ((nint)_get_undo_action_label == 0) throw new System.EntryPointNotFoundException("'get_undo_action_label' is not available for the loaded IDA SDK version.");
        return _get_undo_action_label(p0);
    }

    public static byte* @get_user_idadir()
    {
        if ((nint)_get_user_idadir == 0) throw new System.EntryPointNotFoundException("'get_user_idadir' is not available for the loaded IDA SDK version.");
        return _get_user_idadir();
    }

    public static uint @get_utf8_char(byte** p0)
    {
        if ((nint)_get_utf8_char == 0) throw new System.EntryPointNotFoundException("'get_utf8_char' is not available for the loaded IDA SDK version.");
        return _get_utf8_char(p0);
    }

    public static ulong @get_vftable_ea(uint p0)
    {
        if ((nint)_get_vftable_ea == 0) throw new System.EntryPointNotFoundException("'get_vftable_ea' is not available for the loaded IDA SDK version.");
        return _get_vftable_ea(p0);
    }

    public static uint @get_vftable_ordinal(ulong p0)
    {
        if ((nint)_get_vftable_ordinal == 0) throw new System.EntryPointNotFoundException("'get_vftable_ordinal' is not available for the loaded IDA SDK version.");
        return _get_vftable_ordinal(p0);
    }

    public static ulong @get_wide_byte(ulong p0)
    {
        if ((nint)_get_wide_byte == 0) throw new System.EntryPointNotFoundException("'get_wide_byte' is not available for the loaded IDA SDK version.");
        return _get_wide_byte(p0);
    }

    public static ulong @get_wide_dword(ulong p0)
    {
        if ((nint)_get_wide_dword == 0) throw new System.EntryPointNotFoundException("'get_wide_dword' is not available for the loaded IDA SDK version.");
        return _get_wide_dword(p0);
    }

    public static ulong @get_wide_word(ulong p0)
    {
        if ((nint)_get_wide_word == 0) throw new System.EntryPointNotFoundException("'get_wide_word' is not available for the loaded IDA SDK version.");
        return _get_wide_word(p0);
    }

    public static ushort @get_word(ulong p0)
    {
        if ((nint)_get_word == 0) throw new System.EntryPointNotFoundException("'get_word' is not available for the loaded IDA SDK version.");
        return _get_word(p0);
    }

    public static nint @get_xrefpos(void* p0, ulong p1)
    {
        if ((nint)_get_xrefpos == 0) throw new System.EntryPointNotFoundException("'get_xrefpos' is not available for the loaded IDA SDK version.");
        return _get_xrefpos(p0, p1);
    }

    public static byte @get_zero_ranges(void* p0, void* p1)
    {
        if ((nint)_get_zero_ranges == 0) throw new System.EntryPointNotFoundException("'get_zero_ranges' is not available for the loaded IDA SDK version.");
        return _get_zero_ranges(p0, p1);
    }

    public static nuint @getinf(int p0)
    {
        if ((nint)_getinf == 0) throw new System.EntryPointNotFoundException("'getinf' is not available for the loaded IDA SDK version.");
        return _getinf(p0);
    }

    public static nint @getinf_buf(int p0, void* p1, nuint p2)
    {
        if ((nint)_getinf_buf == 0) throw new System.EntryPointNotFoundException("'getinf_buf' is not available for the loaded IDA SDK version.");
        return _getinf_buf(p0, p1, p2);
    }

    public static byte @getinf_flag(int p0, uint p1)
    {
        if ((nint)_getinf_flag == 0) throw new System.EntryPointNotFoundException("'getinf_flag' is not available for the loaded IDA SDK version.");
        return _getinf_flag(p0, p1);
    }

    public static nint @getinf_str(QString* p0, int p1)
    {
        if ((nint)_getinf_str == 0) throw new System.EntryPointNotFoundException("'getinf_str' is not available for the loaded IDA SDK version.");
        return _getinf_str(p0, p1);
    }

    public static void* @getn_fchunk(int p0)
    {
        if ((nint)_getn_fchunk == 0) throw new System.EntryPointNotFoundException("'getn_fchunk' is not available for the loaded IDA SDK version.");
        return _getn_fchunk(p0);
    }

    public static void* @getn_func(nuint p0)
    {
        if ((nint)_getn_func == 0) throw new System.EntryPointNotFoundException("'getn_func' is not available for the loaded IDA SDK version.");
        return _getn_func(p0);
    }

    public static void* @getn_hidden_range(int p0)
    {
        if ((nint)_getn_hidden_range == 0) throw new System.EntryPointNotFoundException("'getn_hidden_range' is not available for the loaded IDA SDK version.");
        return _getn_hidden_range(p0);
    }

    public static byte @getn_selector(ulong* p0, ulong* p1, int p2)
    {
        if ((nint)_getn_selector == 0) throw new System.EntryPointNotFoundException("'getn_selector' is not available for the loaded IDA SDK version.");
        return _getn_selector(p0, p1, p2);
    }

    public static byte @getn_sourcefile(void* p0, nuint p1)
    {
        if ((nint)_getn_sourcefile == 0) throw new System.EntryPointNotFoundException("'getn_sourcefile' is not available for the loaded IDA SDK version.");
        return _getn_sourcefile(p0, p1);
    }

    public static byte @getn_sreg_range(void* p0, int p1, int p2)
    {
        if ((nint)_getn_sreg_range == 0) throw new System.EntryPointNotFoundException("'getn_sreg_range' is not available for the loaded IDA SDK version.");
        return _getn_sreg_range(p0, p1, p2);
    }

    public static void* @getnseg(int p0)
    {
        if ((nint)_getnseg == 0) throw new System.EntryPointNotFoundException("'getnseg' is not available for the loaded IDA SDK version.");
        return _getnseg(p0);
    }

    public static void* @getseg(ulong p0)
    {
        if ((nint)_getseg == 0) throw new System.EntryPointNotFoundException("'getseg' is not available for the loaded IDA SDK version.");
        return _getseg(p0);
    }

    public static byte* @getsysfile(byte* p0, nuint p1, byte* p2, byte* p3)
    {
        if ((nint)_getsysfile == 0) throw new System.EntryPointNotFoundException("'getsysfile' is not available for the loaded IDA SDK version.");
        return _getsysfile(p0, p1, p2, p3);
    }

    public static uint @guess_func_cc(void* p0, int p1, int p2)
    {
        if ((nint)_guess_func_cc == 0) throw new System.EntryPointNotFoundException("'guess_func_cc' is not available for the loaded IDA SDK version.");
        return _guess_func_cc(p0, p1, p2);
    }

    public static int @guess_tinfo(TypeInfo* p0, ulong p1)
    {
        if ((nint)_guess_tinfo == 0) throw new System.EntryPointNotFoundException("'guess_tinfo' is not available for the loaded IDA SDK version.");
        return _guess_tinfo(p0, p1);
    }

    public static int @h2ti(void* p0, void* p1, byte* p2, int p3, void* p4, void* p5, void* p6, void* p7, int p8)
    {
        if ((nint)_h2ti == 0) throw new System.EntryPointNotFoundException("'h2ti' is not available for the loaded IDA SDK version.");
        return _h2ti(p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static byte @handle_fixups_in_macro(void* p0, ulong p1, ushort p2, uint p3)
    {
        if ((nint)_handle_fixups_in_macro == 0) throw new System.EntryPointNotFoundException("'handle_fixups_in_macro' is not available for the loaded IDA SDK version.");
        return _handle_fixups_in_macro(p0, p1, p2, p3);
    }

    public static byte @has_backup_metadata(ulong p0)
    {
        if ((nint)_has_backup_metadata == 0) throw new System.EntryPointNotFoundException("'has_backup_metadata' is not available for the loaded IDA SDK version.");
        return _has_backup_metadata(p0);
    }

    public static byte @has_external_refs(void* p0, ulong p1)
    {
        if ((nint)_has_external_refs == 0) throw new System.EntryPointNotFoundException("'has_external_refs' is not available for the loaded IDA SDK version.");
        return _has_external_refs(p0, p1);
    }

    public static byte @has_external_refs_ea(ulong p0, ulong p1)
    {
        if ((nint)_has_external_refs_ea == 0) throw new System.EntryPointNotFoundException("'has_external_refs_ea' is not available for the loaded IDA SDK version.");
        return _has_external_refs_ea(p0, p1);
    }

    public static byte @has_insn_feature(ushort p0, uint p1)
    {
        if ((nint)_has_insn_feature == 0) throw new System.EntryPointNotFoundException("'has_insn_feature' is not available for the loaded IDA SDK version.");
        return _has_insn_feature(p0, p1);
    }

    public static byte @has_jump_or_flow_xref(ulong p0)
    {
        if ((nint)_has_jump_or_flow_xref == 0) throw new System.EntryPointNotFoundException("'has_jump_or_flow_xref' is not available for the loaded IDA SDK version.");
        return _has_jump_or_flow_xref(p0);
    }

    public static nuint @hexplace_t__ea2str(byte* p0, nuint p1, void* p2, ulong p3)
    {
        if ((nint)_hexplace_t__ea2str == 0) throw new System.EntryPointNotFoundException("'hexplace_t__ea2str' is not available for the loaded IDA SDK version.");
        return _hexplace_t__ea2str(p0, p1, p2, p3);
    }

    public static void @hexplace_t__out_one_item(void* p0, void* p1, void* p2, int p3, byte* p4, byte p5)
    {
        if ((nint)_hexplace_t__out_one_item == 0) throw new System.EntryPointNotFoundException("'hexplace_t__out_one_item' is not available for the loaded IDA SDK version.");
        _hexplace_t__out_one_item(p0, p1, p2, p3, p4, p5);
    }

    public static void @hide_name(ulong p0)
    {
        if ((nint)_hide_name == 0) throw new System.EntryPointNotFoundException("'hide_name' is not available for the loaded IDA SDK version.");
        _hide_name(p0);
    }

    public static byte @hook_event_listener(int p0, void* p1, void* p2, int p3)
    {
        if ((nint)_hook_event_listener == 0) throw new System.EntryPointNotFoundException("'hook_event_listener' is not available for the loaded IDA SDK version.");
        return _hook_event_listener(p0, p1, p2, p3);
    }

    public static byte @hook_to_notification_point(int p0, void* p1, void* p2)
    {
        if ((nint)_hook_to_notification_point == 0) throw new System.EntryPointNotFoundException("'hook_to_notification_point' is not available for the loaded IDA SDK version.");
        return _hook_to_notification_point(p0, p1, p2);
    }

    public static void @ida_checkmem(byte* p0, int p1)
    {
        if ((nint)_ida_checkmem == 0) throw new System.EntryPointNotFoundException("'ida_checkmem' is not available for the loaded IDA SDK version.");
        _ida_checkmem(p0, p1);
    }

    public static byte* @idadir(byte* p0)
    {
        if ((nint)_idadir == 0) throw new System.EntryPointNotFoundException("'idadir' is not available for the loaded IDA SDK version.");
        return _idadir(p0);
    }

    public static byte @idb_utf8(QString* p0, byte* p1, int p2, int p3)
    {
        if ((nint)_idb_utf8 == 0) throw new System.EntryPointNotFoundException("'idb_utf8' is not available for the loaded IDA SDK version.");
        return _idb_utf8(p0, p1, p2, p3);
    }

    public static int @idcv_float(void* p0)
    {
        if ((nint)_idcv_float == 0) throw new System.EntryPointNotFoundException("'idcv_float' is not available for the loaded IDA SDK version.");
        return _idcv_float(p0);
    }

    public static int @idcv_int64(void* p0)
    {
        if ((nint)_idcv_int64 == 0) throw new System.EntryPointNotFoundException("'idcv_int64' is not available for the loaded IDA SDK version.");
        return _idcv_int64(p0);
    }

    public static int @idcv_long(void* p0)
    {
        if ((nint)_idcv_long == 0) throw new System.EntryPointNotFoundException("'idcv_long' is not available for the loaded IDA SDK version.");
        return _idcv_long(p0);
    }

    public static int @idcv_num(void* p0)
    {
        if ((nint)_idcv_num == 0) throw new System.EntryPointNotFoundException("'idcv_num' is not available for the loaded IDA SDK version.");
        return _idcv_num(p0);
    }

    public static int @idcv_object(void* p0, void* p1)
    {
        if ((nint)_idcv_object == 0) throw new System.EntryPointNotFoundException("'idcv_object' is not available for the loaded IDA SDK version.");
        return _idcv_object(p0, p1);
    }

    public static int @idcv_string(void* p0)
    {
        if ((nint)_idcv_string == 0) throw new System.EntryPointNotFoundException("'idcv_string' is not available for the loaded IDA SDK version.");
        return _idcv_string(p0);
    }

    public static int @ieee2cpu(void* p0, void* p1, int p2)
    {
        if ((nint)_ieee2cpu == 0) throw new System.EntryPointNotFoundException("'ieee2cpu' is not available for the loaded IDA SDK version.");
        return _ieee2cpu(p0, p1, p2);
    }

    public static int @ieee_realcvt(void* p0, void* p1, ushort p2)
    {
        if ((nint)_ieee_realcvt == 0) throw new System.EntryPointNotFoundException("'ieee_realcvt' is not available for the loaded IDA SDK version.");
        return _ieee_realcvt(p0, p1, p2);
    }

    public static void @import_module(byte* p0, byte* p1, ulong p2, void* p3, byte* p4)
    {
        if ((nint)_import_module == 0) throw new System.EntryPointNotFoundException("'import_module' is not available for the loaded IDA SDK version.");
        _import_module(p0, p1, p2, p3, p4);
    }

    public static byte @indexer_is_enabled()
    {
        if ((nint)_indexer_is_enabled == 0) throw new System.EntryPointNotFoundException("'indexer_is_enabled' is not available for the loaded IDA SDK version.");
        return _indexer_is_enabled();
    }

    public static void* @indexer_match(nuint p0, QString* p1, void* p2)
    {
        if ((nint)_indexer_match == 0) throw new System.EntryPointNotFoundException("'indexer_match' is not available for the loaded IDA SDK version.");
        return _indexer_match(p0, p1, p2);
    }

    public static void* @indexer_match_all(QString* p0, void* p1)
    {
        if ((nint)_indexer_match_all == 0) throw new System.EntryPointNotFoundException("'indexer_match_all' is not available for the loaded IDA SDK version.");
        return _indexer_match_all(p0, p1);
    }

    public static int @init_database(int p0, byte** p1, int* p2)
    {
        if ((nint)_init_database == 0) throw new System.EntryPointNotFoundException("'init_database' is not available for the loaded IDA SDK version.");
        return _init_database(p0, p1, p2);
    }

    public static void @init_plugins(int p0)
    {
        if ((nint)_init_plugins == 0) throw new System.EntryPointNotFoundException("'init_plugins' is not available for the loaded IDA SDK version.");
        _init_plugins(p0);
    }

    public static void @insn_add_cref(void* p0, ulong p1, int p2, int p3)
    {
        if ((nint)_insn_add_cref == 0) throw new System.EntryPointNotFoundException("'insn_add_cref' is not available for the loaded IDA SDK version.");
        _insn_add_cref(p0, p1, p2, p3);
    }

    public static void @insn_add_dref(void* p0, ulong p1, int p2, int p3)
    {
        if ((nint)_insn_add_dref == 0) throw new System.EntryPointNotFoundException("'insn_add_dref' is not available for the loaded IDA SDK version.");
        _insn_add_dref(p0, p1, p2, p3);
    }

    public static ulong @insn_add_off_drefs(void* p0, void* p1, int p2, int p3)
    {
        if ((nint)_insn_add_off_drefs == 0) throw new System.EntryPointNotFoundException("'insn_add_off_drefs' is not available for the loaded IDA SDK version.");
        return _insn_add_off_drefs(p0, p1, p2, p3);
    }

    public static byte @insn_create_op_data(void* p0, ulong p1, int p2, byte p3)
    {
        if ((nint)_insn_create_op_data == 0) throw new System.EntryPointNotFoundException("'insn_create_op_data' is not available for the loaded IDA SDK version.");
        return _insn_create_op_data(p0, p1, p2, p3);
    }

    public static byte @insn_create_stkvar(void* p0, void* p1, long p2, int p3)
    {
        if ((nint)_insn_create_stkvar == 0) throw new System.EntryPointNotFoundException("'insn_create_stkvar' is not available for the loaded IDA SDK version.");
        return _insn_create_stkvar(p0, p1, p2, p3);
    }

    public static byte @insn_get_next_byte(void* p0)
    {
        if ((nint)_insn_get_next_byte == 0) throw new System.EntryPointNotFoundException("'insn_get_next_byte' is not available for the loaded IDA SDK version.");
        return _insn_get_next_byte(p0);
    }

    public static uint @insn_get_next_dword(void* p0)
    {
        if ((nint)_insn_get_next_dword == 0) throw new System.EntryPointNotFoundException("'insn_get_next_dword' is not available for the loaded IDA SDK version.");
        return _insn_get_next_dword(p0);
    }

    public static ulong @insn_get_next_qword(void* p0)
    {
        if ((nint)_insn_get_next_qword == 0) throw new System.EntryPointNotFoundException("'insn_get_next_qword' is not available for the loaded IDA SDK version.");
        return _insn_get_next_qword(p0);
    }

    public static ushort @insn_get_next_word(void* p0)
    {
        if ((nint)_insn_get_next_word == 0) throw new System.EntryPointNotFoundException("'insn_get_next_word' is not available for the loaded IDA SDK version.");
        return _insn_get_next_word(p0);
    }

    public static int @install_custom_argloc(void* p0)
    {
        if ((nint)_install_custom_argloc == 0) throw new System.EntryPointNotFoundException("'install_custom_argloc' is not available for the loaded IDA SDK version.");
        return _install_custom_argloc(p0);
    }

    public static nint @install_extlang(void* p0)
    {
        if ((nint)_install_extlang == 0) throw new System.EntryPointNotFoundException("'install_extlang' is not available for the loaded IDA SDK version.");
        return _install_extlang(p0);
    }

    public static byte @install_user_defined_prefix(nuint p0, void* p1, void* p2)
    {
        if ((nint)_install_user_defined_prefix == 0) throw new System.EntryPointNotFoundException("'install_user_defined_prefix' is not available for the loaded IDA SDK version.");
        return _install_user_defined_prefix(p0, p1, p2);
    }

    public static int @internal_register_place_class(void* p0, int p1, void* p2, int p3)
    {
        if ((nint)_internal_register_place_class == 0) throw new System.EntryPointNotFoundException("'internal_register_place_class' is not available for the loaded IDA SDK version.");
        return _internal_register_place_class(p0, p1, p2, p3);
    }

    public static void @interr(int p0)
    {
        if ((nint)_interr == 0) throw new System.EntryPointNotFoundException("'interr' is not available for the loaded IDA SDK version.");
        _interr(p0);
    }

    public static void @invalidate_dbgmem_config()
    {
        if ((nint)_invalidate_dbgmem_config == 0) throw new System.EntryPointNotFoundException("'invalidate_dbgmem_config' is not available for the loaded IDA SDK version.");
        _invalidate_dbgmem_config();
    }

    public static void @invalidate_dbgmem_contents(ulong p0, ulong p1)
    {
        if ((nint)_invalidate_dbgmem_contents == 0) throw new System.EntryPointNotFoundException("'invalidate_dbgmem_contents' is not available for the loaded IDA SDK version.");
        _invalidate_dbgmem_contents(p0, p1);
    }

    public static void @invalidate_regfinder_cache(ulong p0, ulong p1, int p2)
    {
        if ((nint)_invalidate_regfinder_cache == 0) throw new System.EntryPointNotFoundException("'invalidate_regfinder_cache' is not available for the loaded IDA SDK version.");
        _invalidate_regfinder_cache(p0, p1, p2);
    }

    public static void @invalidate_regfinder_xrefs_cache(ulong p0, int p1)
    {
        if ((nint)_invalidate_regfinder_xrefs_cache == 0) throw new System.EntryPointNotFoundException("'invalidate_regfinder_xrefs_cache' is not available for the loaded IDA SDK version.");
        _invalidate_regfinder_xrefs_cache(p0, p1);
    }

    public static byte @invoke_plugin(void* p0)
    {
        if ((nint)_invoke_plugin == 0) throw new System.EntryPointNotFoundException("'invoke_plugin' is not available for the loaded IDA SDK version.");
        return _invoke_plugin(p0);
    }

    public static int @is_align_insn(ulong p0)
    {
        if ((nint)_is_align_insn == 0) throw new System.EntryPointNotFoundException("'is_align_insn' is not available for the loaded IDA SDK version.");
        return _is_align_insn(p0);
    }

    public static byte @is_attached_custom_data_format(int p0, int p1)
    {
        if ((nint)_is_attached_custom_data_format == 0) throw new System.EntryPointNotFoundException("'is_attached_custom_data_format' is not available for the loaded IDA SDK version.");
        return _is_attached_custom_data_format(p0, p1);
    }

    public static byte @is_auto_enabled()
    {
        if ((nint)_is_auto_enabled == 0) throw new System.EntryPointNotFoundException("'is_auto_enabled' is not available for the loaded IDA SDK version.");
        return _is_auto_enabled();
    }

    public static byte @is_basic_block_end(void* p0, byte p1)
    {
        if ((nint)_is_basic_block_end == 0) throw new System.EntryPointNotFoundException("'is_basic_block_end' is not available for the loaded IDA SDK version.");
        return _is_basic_block_end(p0, p1);
    }

    public static byte @is_bnot(ulong p0, ulong p1, int p2)
    {
        if ((nint)_is_bnot == 0) throw new System.EntryPointNotFoundException("'is_bnot' is not available for the loaded IDA SDK version.");
        return _is_bnot(p0, p1, p2);
    }

    public static byte @is_c_keyword(byte* p0)
    {
        if ((nint)_is_c_keyword == 0) throw new System.EntryPointNotFoundException("'is_c_keyword' is not available for the loaded IDA SDK version.");
        return _is_c_keyword(p0);
    }

    public static byte @is_call_insn(void* p0)
    {
        if ((nint)_is_call_insn == 0) throw new System.EntryPointNotFoundException("'is_call_insn' is not available for the loaded IDA SDK version.");
        return _is_call_insn(p0);
    }

    public static byte @is_char(ulong p0, int p1)
    {
        if ((nint)_is_char == 0) throw new System.EntryPointNotFoundException("'is_char' is not available for the loaded IDA SDK version.");
        return _is_char(p0, p1);
    }

    public static int @is_control_tty(int p0)
    {
        if ((nint)_is_control_tty == 0) throw new System.EntryPointNotFoundException("'is_control_tty' is not available for the loaded IDA SDK version.");
        return _is_control_tty(p0);
    }

    public static byte @is_cp_graphical(uint p0)
    {
        if ((nint)_is_cp_graphical == 0) throw new System.EntryPointNotFoundException("'is_cp_graphical' is not available for the loaded IDA SDK version.");
        return _is_cp_graphical(p0);
    }

    public static byte @is_custfmt(ulong p0, int p1)
    {
        if ((nint)_is_custfmt == 0) throw new System.EntryPointNotFoundException("'is_custfmt' is not available for the loaded IDA SDK version.");
        return _is_custfmt(p0, p1);
    }

    public static byte @is_cvt64()
    {
        if ((nint)_is_cvt64 == 0) throw new System.EntryPointNotFoundException("'is_cvt64' is not available for the loaded IDA SDK version.");
        return _is_cvt64();
    }

    public static byte @is_database_busy()
    {
        if ((nint)_is_database_busy == 0) throw new System.EntryPointNotFoundException("'is_database_busy' is not available for the loaded IDA SDK version.");
        return _is_database_busy();
    }

    public static byte @is_database_ext(byte* p0)
    {
        if ((nint)_is_database_ext == 0) throw new System.EntryPointNotFoundException("'is_database_ext' is not available for the loaded IDA SDK version.");
        return _is_database_ext(p0);
    }

    public static byte @is_database_flag(uint p0)
    {
        if ((nint)_is_database_flag == 0) throw new System.EntryPointNotFoundException("'is_database_flag' is not available for the loaded IDA SDK version.");
        return _is_database_flag(p0);
    }

    public static byte @is_debugger_memory(ulong p0)
    {
        if ((nint)_is_debugger_memory == 0) throw new System.EntryPointNotFoundException("'is_debugger_memory' is not available for the loaded IDA SDK version.");
        return _is_debugger_memory(p0);
    }

    public static byte @is_debugger_on()
    {
        if ((nint)_is_debugger_on == 0) throw new System.EntryPointNotFoundException("'is_debugger_on' is not available for the loaded IDA SDK version.");
        return _is_debugger_on();
    }

    public static byte @is_defarg(ulong p0, int p1)
    {
        if ((nint)_is_defarg == 0) throw new System.EntryPointNotFoundException("'is_defarg' is not available for the loaded IDA SDK version.");
        return _is_defarg(p0, p1);
    }

    public static byte @is_diff_merge_mode()
    {
        if ((nint)_is_diff_merge_mode == 0) throw new System.EntryPointNotFoundException("'is_diff_merge_mode' is not available for the loaded IDA SDK version.");
        return _is_diff_merge_mode();
    }

    public static byte @is_ea_tryblks(ulong p0, uint p1)
    {
        if ((nint)_is_ea_tryblks == 0) throw new System.EntryPointNotFoundException("'is_ea_tryblks' is not available for the loaded IDA SDK version.");
        return _is_ea_tryblks(p0, p1);
    }

    public static byte @is_enum(ulong p0, int p1)
    {
        if ((nint)_is_enum == 0) throw new System.EntryPointNotFoundException("'is_enum' is not available for the loaded IDA SDK version.");
        return _is_enum(p0, p1);
    }

    public static byte @is_fltnum(ulong p0, int p1)
    {
        if ((nint)_is_fltnum == 0) throw new System.EntryPointNotFoundException("'is_fltnum' is not available for the loaded IDA SDK version.");
        return _is_fltnum(p0, p1);
    }

    public static byte @is_forced_operand(ulong p0, int p1)
    {
        if ((nint)_is_forced_operand == 0) throw new System.EntryPointNotFoundException("'is_forced_operand' is not available for the loaded IDA SDK version.");
        return _is_forced_operand(p0, p1);
    }

    public static byte @is_func_locked(void* p0)
    {
        if ((nint)_is_func_locked == 0) throw new System.EntryPointNotFoundException("'is_func_locked' is not available for the loaded IDA SDK version.");
        return _is_func_locked(p0);
    }

    public static byte @is_func_locked_ea(ulong p0)
    {
        if ((nint)_is_func_locked_ea == 0) throw new System.EntryPointNotFoundException("'is_func_locked_ea' is not available for the loaded IDA SDK version.");
        return _is_func_locked_ea(p0);
    }

    public static byte @is_function_entry(ulong p0)
    {
        if ((nint)_is_function_entry == 0) throw new System.EntryPointNotFoundException("'is_function_entry' is not available for the loaded IDA SDK version.");
        return _is_function_entry(p0);
    }

    public static byte @is_function_tail(ulong p0)
    {
        if ((nint)_is_function_tail == 0) throw new System.EntryPointNotFoundException("'is_function_tail' is not available for the loaded IDA SDK version.");
        return _is_function_tail(p0);
    }

    public static byte @is_ident(byte* p0)
    {
        if ((nint)_is_ident == 0) throw new System.EntryPointNotFoundException("'is_ident' is not available for the loaded IDA SDK version.");
        return _is_ident(p0);
    }

    public static byte @is_in_nlist(ulong p0)
    {
        if ((nint)_is_in_nlist == 0) throw new System.EntryPointNotFoundException("'is_in_nlist' is not available for the loaded IDA SDK version.");
        return _is_in_nlist(p0);
    }

    public static byte @is_indirect_jump_insn(void* p0)
    {
        if ((nint)_is_indirect_jump_insn == 0) throw new System.EntryPointNotFoundException("'is_indirect_jump_insn' is not available for the loaded IDA SDK version.");
        return _is_indirect_jump_insn(p0);
    }

    public static byte @is_invsign(ulong p0, ulong p1, int p2)
    {
        if ((nint)_is_invsign == 0) throw new System.EntryPointNotFoundException("'is_invsign' is not available for the loaded IDA SDK version.");
        return _is_invsign(p0, p1, p2);
    }

    public static byte @is_loaded(ulong p0)
    {
        if ((nint)_is_loaded == 0) throw new System.EntryPointNotFoundException("'is_loaded' is not available for the loaded IDA SDK version.");
        return _is_loaded(p0);
    }

    public static byte @is_lzero(ulong p0, int p1)
    {
        if ((nint)_is_lzero == 0) throw new System.EntryPointNotFoundException("'is_lzero' is not available for the loaded IDA SDK version.");
        return _is_lzero(p0, p1);
    }

    public static byte @is_main_thread()
    {
        if ((nint)_is_main_thread == 0) throw new System.EntryPointNotFoundException("'is_main_thread' is not available for the loaded IDA SDK version.");
        return _is_main_thread();
    }

    public static byte @is_manual(ulong p0, int p1)
    {
        if ((nint)_is_manual == 0) throw new System.EntryPointNotFoundException("'is_manual' is not available for the loaded IDA SDK version.");
        return _is_manual(p0, p1);
    }

    public static byte @is_manual_insn(ulong p0)
    {
        if ((nint)_is_manual_insn == 0) throw new System.EntryPointNotFoundException("'is_manual_insn' is not available for the loaded IDA SDK version.");
        return _is_manual_insn(p0);
    }

    public static byte @is_mapped(ulong p0)
    {
        if ((nint)_is_mapped == 0) throw new System.EntryPointNotFoundException("'is_mapped' is not available for the loaded IDA SDK version.");
        return _is_mapped(p0);
    }

    public static byte @is_miniidb()
    {
        if ((nint)_is_miniidb == 0) throw new System.EntryPointNotFoundException("'is_miniidb' is not available for the loaded IDA SDK version.");
        return _is_miniidb();
    }

    public static byte @is_name_defined_locally(void* p0, byte* p1, int p2, ulong p3, ulong p4)
    {
        if ((nint)_is_name_defined_locally == 0) throw new System.EntryPointNotFoundException("'is_name_defined_locally' is not available for the loaded IDA SDK version.");
        return _is_name_defined_locally(p0, p1, p2, p3, p4);
    }

    public static byte @is_name_defined_locally_ea(ulong p0, byte* p1, int p2, ulong p3, ulong p4)
    {
        if ((nint)_is_name_defined_locally_ea == 0) throw new System.EntryPointNotFoundException("'is_name_defined_locally_ea' is not available for the loaded IDA SDK version.");
        return _is_name_defined_locally_ea(p0, p1, p2, p3, p4);
    }

    public static byte @is_numop(ulong p0, int p1)
    {
        if ((nint)_is_numop == 0) throw new System.EntryPointNotFoundException("'is_numop' is not available for the loaded IDA SDK version.");
        return _is_numop(p0, p1);
    }

    public static byte @is_numop0(ulong p0)
    {
        if ((nint)_is_numop0 == 0) throw new System.EntryPointNotFoundException("'is_numop0' is not available for the loaded IDA SDK version.");
        return _is_numop0(p0);
    }

    public static byte @is_numop1(ulong p0)
    {
        if ((nint)_is_numop1 == 0) throw new System.EntryPointNotFoundException("'is_numop1' is not available for the loaded IDA SDK version.");
        return _is_numop1(p0);
    }

    public static byte @is_off(ulong p0, int p1)
    {
        if ((nint)_is_off == 0) throw new System.EntryPointNotFoundException("'is_off' is not available for the loaded IDA SDK version.");
        return _is_off(p0, p1);
    }

    public static byte @is_ordinal_name(byte* p0, uint* p1)
    {
        if ((nint)_is_ordinal_name == 0) throw new System.EntryPointNotFoundException("'is_ordinal_name' is not available for the loaded IDA SDK version.");
        return _is_ordinal_name(p0, p1);
    }

    public static byte @is_problem_present(byte p0, ulong p1)
    {
        if ((nint)_is_problem_present == 0) throw new System.EntryPointNotFoundException("'is_problem_present' is not available for the loaded IDA SDK version.");
        return _is_problem_present(p0, p1);
    }

    public static byte @is_public_name(ulong p0)
    {
        if ((nint)_is_public_name == 0) throw new System.EntryPointNotFoundException("'is_public_name' is not available for the loaded IDA SDK version.");
        return _is_public_name(p0);
    }

    public static byte @is_refresh_requested(ulong p0)
    {
        if ((nint)_is_refresh_requested == 0) throw new System.EntryPointNotFoundException("'is_refresh_requested' is not available for the loaded IDA SDK version.");
        return _is_refresh_requested(p0);
    }

    public static byte @is_ret_insn(void* p0, byte p1)
    {
        if ((nint)_is_ret_insn == 0) throw new System.EntryPointNotFoundException("'is_ret_insn' is not available for the loaded IDA SDK version.");
        return _is_ret_insn(p0, p1);
    }

    public static byte @is_same_fchunk(ulong p0, ulong p1)
    {
        if ((nint)_is_same_fchunk == 0) throw new System.EntryPointNotFoundException("'is_same_fchunk' is not available for the loaded IDA SDK version.");
        return _is_same_fchunk(p0, p1);
    }

    public static byte @is_same_segment(ulong p0, ulong p1)
    {
        if ((nint)_is_same_segment == 0) throw new System.EntryPointNotFoundException("'is_same_segment' is not available for the loaded IDA SDK version.");
        return _is_same_segment(p0, p1);
    }

    public static byte @is_seg(ulong p0, int p1)
    {
        if ((nint)_is_seg == 0) throw new System.EntryPointNotFoundException("'is_seg' is not available for the loaded IDA SDK version.");
        return _is_seg(p0, p1);
    }

    public static byte @is_segm_locked(void* p0)
    {
        if ((nint)_is_segm_locked == 0) throw new System.EntryPointNotFoundException("'is_segm_locked' is not available for the loaded IDA SDK version.");
        return _is_segm_locked(p0);
    }

    public static byte @is_segment_locked(ulong p0)
    {
        if ((nint)_is_segment_locked == 0) throw new System.EntryPointNotFoundException("'is_segment_locked' is not available for the loaded IDA SDK version.");
        return _is_segment_locked(p0);
    }

    public static byte @is_spec_ea(ulong p0)
    {
        if ((nint)_is_spec_ea == 0) throw new System.EntryPointNotFoundException("'is_spec_ea' is not available for the loaded IDA SDK version.");
        return _is_spec_ea(p0);
    }

    public static byte @is_spec_segm(byte p0)
    {
        if ((nint)_is_spec_segm == 0) throw new System.EntryPointNotFoundException("'is_spec_segm' is not available for the loaded IDA SDK version.");
        return _is_spec_segm(p0);
    }

    public static byte @is_special_frame_member(ulong p0)
    {
        if ((nint)_is_special_frame_member == 0) throw new System.EntryPointNotFoundException("'is_special_frame_member' is not available for the loaded IDA SDK version.");
        return _is_special_frame_member(p0);
    }

    public static byte @is_stkvar(ulong p0, int p1)
    {
        if ((nint)_is_stkvar == 0) throw new System.EntryPointNotFoundException("'is_stkvar' is not available for the loaded IDA SDK version.");
        return _is_stkvar(p0, p1);
    }

    public static byte @is_stroff(ulong p0, int p1)
    {
        if ((nint)_is_stroff == 0) throw new System.EntryPointNotFoundException("'is_stroff' is not available for the loaded IDA SDK version.");
        return _is_stroff(p0, p1);
    }

    public static byte @is_suspop(ulong p0, ulong p1, int p2)
    {
        if ((nint)_is_suspop == 0) throw new System.EntryPointNotFoundException("'is_suspop' is not available for the loaded IDA SDK version.");
        return _is_suspop(p0, p1, p2);
    }

    public static byte @is_trusted_idb()
    {
        if ((nint)_is_trusted_idb == 0) throw new System.EntryPointNotFoundException("'is_trusted_idb' is not available for the loaded IDA SDK version.");
        return _is_trusted_idb();
    }

    public static byte @is_type_choosable(void* p0, uint p1)
    {
        if ((nint)_is_type_choosable == 0) throw new System.EntryPointNotFoundException("'is_type_choosable' is not available for the loaded IDA SDK version.");
        return _is_type_choosable(p0, p1);
    }

    public static byte @is_uname(byte* p0)
    {
        if ((nint)_is_uname == 0) throw new System.EntryPointNotFoundException("'is_uname' is not available for the loaded IDA SDK version.");
        return _is_uname(p0);
    }

    public static byte @is_valid_cp(uint p0, int p1, void* p2)
    {
        if ((nint)_is_valid_cp == 0) throw new System.EntryPointNotFoundException("'is_valid_cp' is not available for the loaded IDA SDK version.");
        return _is_valid_cp(p0, p1, p2);
    }

    public static byte @is_valid_typename(byte* p0)
    {
        if ((nint)_is_valid_typename == 0) throw new System.EntryPointNotFoundException("'is_valid_typename' is not available for the loaded IDA SDK version.");
        return _is_valid_typename(p0);
    }

    public static byte @is_valid_utf8(byte* p0)
    {
        if ((nint)_is_valid_utf8 == 0) throw new System.EntryPointNotFoundException("'is_valid_utf8' is not available for the loaded IDA SDK version.");
        return _is_valid_utf8(p0);
    }

    public static int @is_varsize_item(ulong p0, ulong p1, void* p2, ulong* p3)
    {
        if ((nint)_is_varsize_item == 0) throw new System.EntryPointNotFoundException("'is_varsize_item' is not available for the loaded IDA SDK version.");
        return _is_varsize_item(p0, p1, p2, p3);
    }

    public static byte @is_weak_name(ulong p0)
    {
        if ((nint)_is_weak_name == 0) throw new System.EntryPointNotFoundException("'is_weak_name' is not available for the loaded IDA SDK version.");
        return _is_weak_name(p0);
    }

    public static void @iterate_func_chunks_ea(ulong p0, void* p1, byte p2)
    {
        if ((nint)_iterate_func_chunks_ea == 0) throw new System.EntryPointNotFoundException("'iterate_func_chunks_ea' is not available for the loaded IDA SDK version.");
        _iterate_func_chunks_ea(p0, p1, p2);
    }

    public static byte* @itext(int p0)
    {
        if ((nint)_itext == 0) throw new System.EntryPointNotFoundException("'itext' is not available for the loaded IDA SDK version.");
        return _itext(p0);
    }

    public static void @jvalue_t_clear(void* p0)
    {
        if ((nint)_jvalue_t_clear == 0) throw new System.EntryPointNotFoundException("'jvalue_t_clear' is not available for the loaded IDA SDK version.");
        _jvalue_t_clear(p0);
    }

    public static void @jvalue_t_copy(void* p0, void* p1)
    {
        if ((nint)_jvalue_t_copy == 0) throw new System.EntryPointNotFoundException("'jvalue_t_copy' is not available for the loaded IDA SDK version.");
        _jvalue_t_copy(p0, p1);
    }

    public static int @l_compare(void* p0, void* p1)
    {
        if ((nint)_l_compare == 0) throw new System.EntryPointNotFoundException("'l_compare' is not available for the loaded IDA SDK version.");
        return _l_compare(p0, p1);
    }

    public static int @l_compare2(void* p0, void* p1, void* p2)
    {
        if ((nint)_l_compare2 == 0) throw new System.EntryPointNotFoundException("'l_compare2' is not available for the loaded IDA SDK version.");
        return _l_compare2(p0, p1, p2);
    }

    public static byte @l_equals(void* p0, void* p1, void* p2)
    {
        if ((nint)_l_equals == 0) throw new System.EntryPointNotFoundException("'l_equals' is not available for the loaded IDA SDK version.");
        return _l_equals(p0, p1, p2);
    }

    public static byte* @last_idcv_attr(void* p0)
    {
        if ((nint)_last_idcv_attr == 0) throw new System.EntryPointNotFoundException("'last_idcv_attr' is not available for the loaded IDA SDK version.");
        return _last_idcv_attr(p0);
    }

    public static void* @launch_process(void* p0, QString* p1)
    {
        if ((nint)_launch_process == 0) throw new System.EntryPointNotFoundException("'launch_process' is not available for the loaded IDA SDK version.");
        return _launch_process(p0, p1);
    }

    public static byte @lcred_process_switch(void* p0, byte* p1)
    {
        if ((nint)_lcred_process_switch == 0) throw new System.EntryPointNotFoundException("'lcred_process_switch' is not available for the loaded IDA SDK version.");
        return _lcred_process_switch(p0, p1);
    }

    public static byte @leading_zero_important(ulong p0, int p1)
    {
        if ((nint)_leading_zero_important == 0) throw new System.EntryPointNotFoundException("'leading_zero_important' is not available for the loaded IDA SDK version.");
        return _leading_zero_important(p0, p1);
    }

    public static int @lex_define_macro(void* p0, byte* p1, byte* p2, int p3, byte p4)
    {
        if ((nint)_lex_define_macro == 0) throw new System.EntryPointNotFoundException("'lex_define_macro' is not available for the loaded IDA SDK version.");
        return _lex_define_macro(p0, p1, p2, p3, p4);
    }

    public static byte* @lex_get_file_line(void* p0, int* p1, byte** p2, int p3)
    {
        if ((nint)_lex_get_file_line == 0) throw new System.EntryPointNotFoundException("'lex_get_file_line' is not available for the loaded IDA SDK version.");
        return _lex_get_file_line(p0, p1, p2, p3);
    }

    public static int @lex_get_token(void* p0, void* p1, int* p2)
    {
        if ((nint)_lex_get_token == 0) throw new System.EntryPointNotFoundException("'lex_get_token' is not available for the loaded IDA SDK version.");
        return _lex_get_token(p0, p1, p2);
    }

    public static int @lex_init_file(void* p0, byte* p1)
    {
        if ((nint)_lex_init_file == 0) throw new System.EntryPointNotFoundException("'lex_init_file' is not available for the loaded IDA SDK version.");
        return _lex_init_file(p0, p1);
    }

    public static int @lex_init_string(void* p0, byte* p1, void* p2)
    {
        if ((nint)_lex_init_string == 0) throw new System.EntryPointNotFoundException("'lex_init_string' is not available for the loaded IDA SDK version.");
        return _lex_init_string(p0, p1, p2);
    }

    public static byte* @lex_print_token(QString* p0, void* p1)
    {
        if ((nint)_lex_print_token == 0) throw new System.EntryPointNotFoundException("'lex_print_token' is not available for the loaded IDA SDK version.");
        return _lex_print_token(p0, p1);
    }

    public static int @lex_set_options(void* p0, int p1)
    {
        if ((nint)_lex_set_options == 0) throw new System.EntryPointNotFoundException("'lex_set_options' is not available for the loaded IDA SDK version.");
        return _lex_set_options(p0, p1);
    }

    public static void @lex_term_file(void* p0, byte p1)
    {
        if ((nint)_lex_term_file == 0) throw new System.EntryPointNotFoundException("'lex_term_file' is not available for the loaded IDA SDK version.");
        _lex_term_file(p0, p1);
    }

    public static void @lex_undefine_macro(void* p0, byte* p1)
    {
        if ((nint)_lex_undefine_macro == 0) throw new System.EntryPointNotFoundException("'lex_undefine_macro' is not available for the loaded IDA SDK version.");
        _lex_undefine_macro(p0, p1);
    }

    public static int @lexcompare_tinfo(ulong p0, ulong p1, int p2)
    {
        if ((nint)_lexcompare_tinfo == 0) throw new System.EntryPointNotFoundException("'lexcompare_tinfo' is not available for the loaded IDA SDK version.");
        return _lexcompare_tinfo(p0, p1, p2);
    }

    public static byte @linearray_t_beginning(void* p0)
    {
        if ((nint)_linearray_t_beginning == 0) throw new System.EntryPointNotFoundException("'linearray_t_beginning' is not available for the loaded IDA SDK version.");
        return _linearray_t_beginning(p0);
    }

    public static void @linearray_t_copy_from(void* p0, void* p1)
    {
        if ((nint)_linearray_t_copy_from == 0) throw new System.EntryPointNotFoundException("'linearray_t_copy_from' is not available for the loaded IDA SDK version.");
        _linearray_t_copy_from(p0, p1);
    }

    public static void @linearray_t_ctr(void* p0, void* p1)
    {
        if ((nint)_linearray_t_ctr == 0) throw new System.EntryPointNotFoundException("'linearray_t_ctr' is not available for the loaded IDA SDK version.");
        _linearray_t_ctr(p0, p1);
    }

    public static QString* @linearray_t_down(void* p0)
    {
        if ((nint)_linearray_t_down == 0) throw new System.EntryPointNotFoundException("'linearray_t_down' is not available for the loaded IDA SDK version.");
        return _linearray_t_down(p0);
    }

    public static void @linearray_t_dtr(void* p0)
    {
        if ((nint)_linearray_t_dtr == 0) throw new System.EntryPointNotFoundException("'linearray_t_dtr' is not available for the loaded IDA SDK version.");
        _linearray_t_dtr(p0);
    }

    public static byte @linearray_t_ending(void* p0)
    {
        if ((nint)_linearray_t_ending == 0) throw new System.EntryPointNotFoundException("'linearray_t_ending' is not available for the loaded IDA SDK version.");
        return _linearray_t_ending(p0);
    }

    public static int @linearray_t_set_place(void* p0, void* p1)
    {
        if ((nint)_linearray_t_set_place == 0) throw new System.EntryPointNotFoundException("'linearray_t_set_place' is not available for the loaded IDA SDK version.");
        return _linearray_t_set_place(p0, p1);
    }

    public static QString* @linearray_t_up(void* p0)
    {
        if ((nint)_linearray_t_up == 0) throw new System.EntryPointNotFoundException("'linearray_t_up' is not available for the loaded IDA SDK version.");
        return _linearray_t_up(p0);
    }

    public static long @llong_scan(byte* p0, int p1, byte** p2)
    {
        if ((nint)_llong_scan == 0) throw new System.EntryPointNotFoundException("'llong_scan' is not available for the loaded IDA SDK version.");
        return _llong_scan(p0, p1, p2);
    }

    public static byte @load_core_module(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_load_core_module == 0) throw new System.EntryPointNotFoundException("'load_core_module' is not available for the loaded IDA SDK version.");
        return _load_core_module(p0, p1, p2);
    }

    public static byte @load_dirtree(void* p0)
    {
        if ((nint)_load_dirtree == 0) throw new System.EntryPointNotFoundException("'load_dirtree' is not available for the loaded IDA SDK version.");
        return _load_dirtree(p0);
    }

    public static int @load_ids_module(byte* p0)
    {
        if ((nint)_load_ids_module == 0) throw new System.EntryPointNotFoundException("'load_ids_module' is not available for the loaded IDA SDK version.");
        return _load_ids_module(p0);
    }

    public static byte @load_nonbinary_file(byte* p0, void* p1, byte* p2, ushort p3, void* p4)
    {
        if ((nint)_load_nonbinary_file == 0) throw new System.EntryPointNotFoundException("'load_nonbinary_file' is not available for the loaded IDA SDK version.");
        return _load_nonbinary_file(p0, p1, p2, p3, p4);
    }

    public static void* @load_til(byte* p0, QString* p1, byte* p2)
    {
        if ((nint)_load_til == 0) throw new System.EntryPointNotFoundException("'load_til' is not available for the loaded IDA SDK version.");
        return _load_til(p0, p1, p2);
    }

    public static void* @load_til_header(byte* p0, byte* p1, QString* p2)
    {
        if ((nint)_load_til_header == 0) throw new System.EntryPointNotFoundException("'load_til_header' is not available for the loaded IDA SDK version.");
        return _load_til_header(p0, p1, p2);
    }

    public static byte @lochist_entry_t_deserialize(void* p0, byte** p1, byte* p2, void* p3)
    {
        if ((nint)_lochist_entry_t_deserialize == 0) throw new System.EntryPointNotFoundException("'lochist_entry_t_deserialize' is not available for the loaded IDA SDK version.");
        return _lochist_entry_t_deserialize(p0, p1, p2, p3);
    }

    public static void @lochist_entry_t_serialize(void* p0, void* p1)
    {
        if ((nint)_lochist_entry_t_serialize == 0) throw new System.EntryPointNotFoundException("'lochist_entry_t_serialize' is not available for the loaded IDA SDK version.");
        _lochist_entry_t_serialize(p0, p1);
    }

    public static byte @lochist_t_back(void* p0, uint p1, byte p2)
    {
        if ((nint)_lochist_t_back == 0) throw new System.EntryPointNotFoundException("'lochist_t_back' is not available for the loaded IDA SDK version.");
        return _lochist_t_back(p0, p1, p2);
    }

    public static void @lochist_t_clear(void* p0)
    {
        if ((nint)_lochist_t_clear == 0) throw new System.EntryPointNotFoundException("'lochist_t_clear' is not available for the loaded IDA SDK version.");
        _lochist_t_clear(p0);
    }

    public static uint @lochist_t_current_index(void* p0)
    {
        if ((nint)_lochist_t_current_index == 0) throw new System.EntryPointNotFoundException("'lochist_t_current_index' is not available for the loaded IDA SDK version.");
        return _lochist_t_current_index(p0);
    }

    public static void @lochist_t_deregister_live(void* p0)
    {
        if ((nint)_lochist_t_deregister_live == 0) throw new System.EntryPointNotFoundException("'lochist_t_deregister_live' is not available for the loaded IDA SDK version.");
        _lochist_t_deregister_live(p0);
    }

    public static byte @lochist_t_fwd(void* p0, uint p1, byte p2)
    {
        if ((nint)_lochist_t_fwd == 0) throw new System.EntryPointNotFoundException("'lochist_t_fwd' is not available for the loaded IDA SDK version.");
        return _lochist_t_fwd(p0, p1, p2);
    }

    public static byte @lochist_t_get(void* p0, void* p1, uint p2)
    {
        if ((nint)_lochist_t_get == 0) throw new System.EntryPointNotFoundException("'lochist_t_get' is not available for the loaded IDA SDK version.");
        return _lochist_t_get(p0, p1, p2);
    }

    public static void* @lochist_t_get_current(void* p0)
    {
        if ((nint)_lochist_t_get_current == 0) throw new System.EntryPointNotFoundException("'lochist_t_get_current' is not available for the loaded IDA SDK version.");
        return _lochist_t_get_current(p0);
    }

    public static byte @lochist_t_init(void* p0, byte* p1, void* p2, void* p3, uint p4)
    {
        if ((nint)_lochist_t_init == 0) throw new System.EntryPointNotFoundException("'lochist_t_init' is not available for the loaded IDA SDK version.");
        return _lochist_t_init(p0, p1, p2, p3, p4);
    }

    public static void @lochist_t_jump(void* p0, byte p1, void* p2)
    {
        if ((nint)_lochist_t_jump == 0) throw new System.EntryPointNotFoundException("'lochist_t_jump' is not available for the loaded IDA SDK version.");
        _lochist_t_jump(p0, p1, p2);
    }

    public static void @lochist_t_register_live(void* p0)
    {
        if ((nint)_lochist_t_register_live == 0) throw new System.EntryPointNotFoundException("'lochist_t_register_live' is not available for the loaded IDA SDK version.");
        _lochist_t_register_live(p0);
    }

    public static void @lochist_t_save(void* p0)
    {
        if ((nint)_lochist_t_save == 0) throw new System.EntryPointNotFoundException("'lochist_t_save' is not available for the loaded IDA SDK version.");
        _lochist_t_save(p0);
    }

    public static byte @lochist_t_seek(void* p0, uint p1, byte p2, byte p3)
    {
        if ((nint)_lochist_t_seek == 0) throw new System.EntryPointNotFoundException("'lochist_t_seek' is not available for the loaded IDA SDK version.");
        return _lochist_t_seek(p0, p1, p2, p3);
    }

    public static void @lochist_t_set(void* p0, uint p1, void* p2)
    {
        if ((nint)_lochist_t_set == 0) throw new System.EntryPointNotFoundException("'lochist_t_set' is not available for the loaded IDA SDK version.");
        _lochist_t_set(p0, p1, p2);
    }

    public static uint @lochist_t_size(void* p0)
    {
        if ((nint)_lochist_t_size == 0) throw new System.EntryPointNotFoundException("'lochist_t_size' is not available for the loaded IDA SDK version.");
        return _lochist_t_size(p0);
    }

    public static void @lock_dbgmem_config()
    {
        if ((nint)_lock_dbgmem_config == 0) throw new System.EntryPointNotFoundException("'lock_dbgmem_config' is not available for the loaded IDA SDK version.");
        _lock_dbgmem_config();
    }

    public static void @lock_func_range(void* p0, byte p1)
    {
        if ((nint)_lock_func_range == 0) throw new System.EntryPointNotFoundException("'lock_func_range' is not available for the loaded IDA SDK version.");
        _lock_func_range(p0, p1);
    }

    public static void @lock_func_range_ea(ulong p0, byte p1)
    {
        if ((nint)_lock_func_range_ea == 0) throw new System.EntryPointNotFoundException("'lock_func_range_ea' is not available for the loaded IDA SDK version.");
        _lock_func_range_ea(p0, p1);
    }

    public static void @lock_segm(void* p0, byte p1)
    {
        if ((nint)_lock_segm == 0) throw new System.EntryPointNotFoundException("'lock_segm' is not available for the loaded IDA SDK version.");
        _lock_segm(p0, p1);
    }

    public static void @lock_segment_by_ea(ulong p0, byte p1)
    {
        if ((nint)_lock_segment_by_ea == 0) throw new System.EntryPointNotFoundException("'lock_segment_by_ea' is not available for the loaded IDA SDK version.");
        _lock_segment_by_ea(p0, p1);
    }

    public static int @log2ceil(ulong p0)
    {
        if ((nint)_log2ceil == 0) throw new System.EntryPointNotFoundException("'log2ceil' is not available for the loaded IDA SDK version.");
        return _log2ceil(p0);
    }

    public static int @log2floor(ulong p0)
    {
        if ((nint)_log2floor == 0) throw new System.EntryPointNotFoundException("'log2floor' is not available for the loaded IDA SDK version.");
        return _log2floor(p0);
    }

    public static void* @lookup_loc_converter2(byte* p0, byte* p1)
    {
        if ((nint)_lookup_loc_converter2 == 0) throw new System.EntryPointNotFoundException("'lookup_loc_converter2' is not available for the loaded IDA SDK version.");
        return _lookup_loc_converter2(p0, p1);
    }

    public static int @lower_type(void* p0, TypeInfo* p1, byte* p2, void* p3)
    {
        if ((nint)_lower_type == 0) throw new System.EntryPointNotFoundException("'lower_type' is not available for the loaded IDA SDK version.");
        return _lower_type(p0, p1, p2, p3);
    }

    public static void @lread(void* p0, void* p1, nuint p2)
    {
        if ((nint)_lread == 0) throw new System.EntryPointNotFoundException("'lread' is not available for the loaded IDA SDK version.");
        _lread(p0, p1, p2);
    }

    public static int @lreadbytes(void* p0, void* p1, nuint p2, byte p3)
    {
        if ((nint)_lreadbytes == 0) throw new System.EntryPointNotFoundException("'lreadbytes' is not available for the loaded IDA SDK version.");
        return _lreadbytes(p0, p1, p2, p3);
    }

    public static void* @make_linput(void* p0)
    {
        if ((nint)_make_linput == 0) throw new System.EntryPointNotFoundException("'make_linput' is not available for the loaded IDA SDK version.");
        return _make_linput(p0);
    }

    public static byte @make_name_auto(ulong p0)
    {
        if ((nint)_make_name_auto == 0) throw new System.EntryPointNotFoundException("'make_name_auto' is not available for the loaded IDA SDK version.");
        return _make_name_auto(p0);
    }

    public static void @make_name_non_public(ulong p0)
    {
        if ((nint)_make_name_non_public == 0) throw new System.EntryPointNotFoundException("'make_name_non_public' is not available for the loaded IDA SDK version.");
        _make_name_non_public(p0);
    }

    public static void @make_name_non_weak(ulong p0)
    {
        if ((nint)_make_name_non_weak == 0) throw new System.EntryPointNotFoundException("'make_name_non_weak' is not available for the loaded IDA SDK version.");
        _make_name_non_weak(p0);
    }

    public static void @make_name_public(ulong p0)
    {
        if ((nint)_make_name_public == 0) throw new System.EntryPointNotFoundException("'make_name_public' is not available for the loaded IDA SDK version.");
        _make_name_public(p0);
    }

    public static byte @make_name_user(ulong p0)
    {
        if ((nint)_make_name_user == 0) throw new System.EntryPointNotFoundException("'make_name_user' is not available for the loaded IDA SDK version.");
        return _make_name_user(p0);
    }

    public static void @make_name_weak(ulong p0)
    {
        if ((nint)_make_name_weak == 0) throw new System.EntryPointNotFoundException("'make_name_weak' is not available for the loaded IDA SDK version.");
        _make_name_weak(p0);
    }

    public static void @make_script_ns(QString* p0, byte* p1, byte* p2)
    {
        if ((nint)_make_script_ns == 0) throw new System.EntryPointNotFoundException("'make_script_ns' is not available for the loaded IDA SDK version.");
        _make_script_ns(p0, p1, p2);
    }

    public static byte @make_signatures(byte p0)
    {
        if ((nint)_make_signatures == 0) throw new System.EntryPointNotFoundException("'make_signatures' is not available for the loaded IDA SDK version.");
        return _make_signatures(p0);
    }

    public static ulong @map_code_ea(void* p0, ulong p1, int p2)
    {
        if ((nint)_map_code_ea == 0) throw new System.EntryPointNotFoundException("'map_code_ea' is not available for the loaded IDA SDK version.");
        return _map_code_ea(p0, p1, p2);
    }

    public static void @mark_switch_insns_jpt(void* p0, int p1, int p2)
    {
        if ((nint)_mark_switch_insns_jpt == 0) throw new System.EntryPointNotFoundException("'mark_switch_insns_jpt' is not available for the loaded IDA SDK version.");
        _mark_switch_insns_jpt(p0, p1, p2);
    }

    public static byte @match_jpt(void* p0)
    {
        if ((nint)_match_jpt == 0) throw new System.EntryPointNotFoundException("'match_jpt' is not available for the loaded IDA SDK version.");
        return _match_jpt(p0);
    }

    public static int @memicmp(void* p0, void* p1, nuint p2)
    {
        if ((nint)_memicmp == 0) throw new System.EntryPointNotFoundException("'memicmp' is not available for the loaded IDA SDK version.");
        return _memicmp(p0, p1, p2);
    }

    public static void* @memrev(void* p0, nint p1)
    {
        if ((nint)_memrev == 0) throw new System.EntryPointNotFoundException("'memrev' is not available for the loaded IDA SDK version.");
        return _memrev(p0, p1);
    }

    public static int @move_idcv(void* p0, void* p1)
    {
        if ((nint)_move_idcv == 0) throw new System.EntryPointNotFoundException("'move_idcv' is not available for the loaded IDA SDK version.");
        return _move_idcv(p0, p1);
    }

    public static byte @move_privrange(ulong p0)
    {
        if ((nint)_move_privrange == 0) throw new System.EntryPointNotFoundException("'move_privrange' is not available for the loaded IDA SDK version.");
        return _move_privrange(p0);
    }

    public static int @move_segm(void* p0, ulong p1, int p2)
    {
        if ((nint)_move_segm == 0) throw new System.EntryPointNotFoundException("'move_segm' is not available for the loaded IDA SDK version.");
        return _move_segm(p0, p1, p2);
    }

    public static byte @move_segm_start(ulong p0, ulong p1, int p2)
    {
        if ((nint)_move_segm_start == 0) throw new System.EntryPointNotFoundException("'move_segm_start' is not available for the loaded IDA SDK version.");
        return _move_segm_start(p0, p1, p2);
    }

    public static byte* @move_segm_strerror(int p0)
    {
        if ((nint)_move_segm_strerror == 0) throw new System.EntryPointNotFoundException("'move_segm_strerror' is not available for the loaded IDA SDK version.");
        return _move_segm_strerror(p0);
    }

    public static int @move_segment(ulong p0, ulong p1, int p2)
    {
        if ((nint)_move_segment == 0) throw new System.EntryPointNotFoundException("'move_segment' is not available for the loaded IDA SDK version.");
        return _move_segment(p0, p1, p2);
    }

    public static byte @name_requires_qualifier(QString* p0, ulong p1, byte* p2, ulong p3)
    {
        if ((nint)_name_requires_qualifier == 0) throw new System.EntryPointNotFoundException("'name_requires_qualifier' is not available for the loaded IDA SDK version.");
        return _name_requires_qualifier(p0, p1, p2, p3);
    }

    public static byte @navstack_entry_t_deserialize(void* p0, byte** p1, byte* p2, void* p3)
    {
        if ((nint)_navstack_entry_t_deserialize == 0) throw new System.EntryPointNotFoundException("'navstack_entry_t_deserialize' is not available for the loaded IDA SDK version.");
        return _navstack_entry_t_deserialize(p0, p1, p2, p3);
    }

    public static void @navstack_entry_t_serialize(void* p0, void* p1)
    {
        if ((nint)_navstack_entry_t_serialize == 0) throw new System.EntryPointNotFoundException("'navstack_entry_t_serialize' is not available for the loaded IDA SDK version.");
        _navstack_entry_t_serialize(p0, p1);
    }

    public static void @navstack_t_deregister_live(void* p0)
    {
        if ((nint)_navstack_t_deregister_live == 0) throw new System.EntryPointNotFoundException("'navstack_t_deregister_live' is not available for the loaded IDA SDK version.");
        _navstack_t_deregister_live(p0);
    }

    public static void @navstack_t_dump(void* p0)
    {
        if ((nint)_navstack_t_dump == 0) throw new System.EntryPointNotFoundException("'navstack_t_dump' is not available for the loaded IDA SDK version.");
        _navstack_t_dump(p0);
    }

    public static void @navstack_t_get_all_current(void* p0, void* p1)
    {
        if ((nint)_navstack_t_get_all_current == 0) throw new System.EntryPointNotFoundException("'navstack_t_get_all_current' is not available for the loaded IDA SDK version.");
        _navstack_t_get_all_current(p0, p1);
    }

    public static byte @navstack_t_get_current(void* p0, void* p1, byte* p2)
    {
        if ((nint)_navstack_t_get_current == 0) throw new System.EntryPointNotFoundException("'navstack_t_get_current' is not available for the loaded IDA SDK version.");
        return _navstack_t_get_current(p0, p1, p2);
    }

    public static byte @navstack_t_get_stack_entry(void* p0, void* p1, uint p2)
    {
        if ((nint)_navstack_t_get_stack_entry == 0) throw new System.EntryPointNotFoundException("'navstack_t_get_stack_entry' is not available for the loaded IDA SDK version.");
        return _navstack_t_get_stack_entry(p0, p1, p2);
    }

    public static byte @navstack_t_init(void* p0, void* p1, byte* p2, uint p3)
    {
        if ((nint)_navstack_t_init == 0) throw new System.EntryPointNotFoundException("'navstack_t_init' is not available for the loaded IDA SDK version.");
        return _navstack_t_init(p0, p1, p2, p3);
    }

    public static byte @navstack_t_perform_move(byte* p0, byte* p1, byte* p2, byte p3)
    {
        if ((nint)_navstack_t_perform_move == 0) throw new System.EntryPointNotFoundException("'navstack_t_perform_move' is not available for the loaded IDA SDK version.");
        return _navstack_t_perform_move(p0, p1, p2, p3);
    }

    public static void @navstack_t_register_live(void* p0)
    {
        if ((nint)_navstack_t_register_live == 0) throw new System.EntryPointNotFoundException("'navstack_t_register_live' is not available for the loaded IDA SDK version.");
        _navstack_t_register_live(p0);
    }

    public static void @navstack_t_set_current(void* p0, void* p1, byte p2)
    {
        if ((nint)_navstack_t_set_current == 0) throw new System.EntryPointNotFoundException("'navstack_t_set_current' is not available for the loaded IDA SDK version.");
        _navstack_t_set_current(p0, p1, p2);
    }

    public static void @navstack_t_set_stack_entry(void* p0, uint p1, void* p2)
    {
        if ((nint)_navstack_t_set_stack_entry == 0) throw new System.EntryPointNotFoundException("'navstack_t_set_stack_entry' is not available for the loaded IDA SDK version.");
        _navstack_t_set_stack_entry(p0, p1, p2);
    }

    public static void @navstack_t_stack_clear(void* p0, void* p1)
    {
        if ((nint)_navstack_t_stack_clear == 0) throw new System.EntryPointNotFoundException("'navstack_t_stack_clear' is not available for the loaded IDA SDK version.");
        _navstack_t_stack_clear(p0, p1);
    }

    public static uint @navstack_t_stack_index(void* p0)
    {
        if ((nint)_navstack_t_stack_index == 0) throw new System.EntryPointNotFoundException("'navstack_t_stack_index' is not available for the loaded IDA SDK version.");
        return _navstack_t_stack_index(p0);
    }

    public static void @navstack_t_stack_jump(void* p0, byte p1, void* p2)
    {
        if ((nint)_navstack_t_stack_jump == 0) throw new System.EntryPointNotFoundException("'navstack_t_stack_jump' is not available for the loaded IDA SDK version.");
        _navstack_t_stack_jump(p0, p1, p2);
    }

    public static byte @navstack_t_stack_nav(void* p0, void* p1, byte p2, uint p3, byte p4)
    {
        if ((nint)_navstack_t_stack_nav == 0) throw new System.EntryPointNotFoundException("'navstack_t_stack_nav' is not available for the loaded IDA SDK version.");
        return _navstack_t_stack_nav(p0, p1, p2, p3, p4);
    }

    public static byte @navstack_t_stack_seek(void* p0, void* p1, uint p2, byte p3, byte p4)
    {
        if ((nint)_navstack_t_stack_seek == 0) throw new System.EntryPointNotFoundException("'navstack_t_stack_seek' is not available for the loaded IDA SDK version.");
        return _navstack_t_stack_seek(p0, p1, p2, p3, p4);
    }

    public static uint @navstack_t_stack_size(void* p0)
    {
        if ((nint)_navstack_t_stack_size == 0) throw new System.EntryPointNotFoundException("'navstack_t_stack_size' is not available for the loaded IDA SDK version.");
        return _navstack_t_stack_size(p0);
    }

    public static int @nbits(ulong p0)
    {
        if ((nint)_nbits == 0) throw new System.EntryPointNotFoundException("'nbits' is not available for the loaded IDA SDK version.");
        return _nbits(p0);
    }

    public static void @netnode_altadjust2(ulong p0, ulong p1, ulong p2, ulong p3, void* p4)
    {
        if ((nint)_netnode_altadjust2 == 0) throw new System.EntryPointNotFoundException("'netnode_altadjust2' is not available for the loaded IDA SDK version.");
        _netnode_altadjust2(p0, p1, p2, p3, p4);
    }

    public static nuint @netnode_altshift(ulong p0, ulong p1, ulong p2, ulong p3, int p4)
    {
        if ((nint)_netnode_altshift == 0) throw new System.EntryPointNotFoundException("'netnode_altshift' is not available for the loaded IDA SDK version.");
        return _netnode_altshift(p0, p1, p2, p3, p4);
    }

    public static ulong @netnode_altval(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_altval == 0) throw new System.EntryPointNotFoundException("'netnode_altval' is not available for the loaded IDA SDK version.");
        return _netnode_altval(p0, p1, p2);
    }

    public static ulong @netnode_altval_idx8(ulong p0, byte p1, int p2)
    {
        if ((nint)_netnode_altval_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_altval_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_altval_idx8(p0, p1, p2);
    }

    public static nuint @netnode_blobshift(ulong p0, ulong p1, ulong p2, ulong p3, int p4)
    {
        if ((nint)_netnode_blobshift == 0) throw new System.EntryPointNotFoundException("'netnode_blobshift' is not available for the loaded IDA SDK version.");
        return _netnode_blobshift(p0, p1, p2, p3, p4);
    }

    public static nuint @netnode_blobsize(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_blobsize == 0) throw new System.EntryPointNotFoundException("'netnode_blobsize' is not available for the loaded IDA SDK version.");
        return _netnode_blobsize(p0, p1, p2);
    }

    public static nuint @netnode_charshift(ulong p0, ulong p1, ulong p2, ulong p3, int p4)
    {
        if ((nint)_netnode_charshift == 0) throw new System.EntryPointNotFoundException("'netnode_charshift' is not available for the loaded IDA SDK version.");
        return _netnode_charshift(p0, p1, p2, p3, p4);
    }

    public static byte @netnode_charval(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_charval == 0) throw new System.EntryPointNotFoundException("'netnode_charval' is not available for the loaded IDA SDK version.");
        return _netnode_charval(p0, p1, p2);
    }

    public static byte @netnode_charval_idx8(ulong p0, byte p1, int p2)
    {
        if ((nint)_netnode_charval_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_charval_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_charval_idx8(p0, p1, p2);
    }

    public static byte @netnode_check(void* p0, byte* p1, nuint p2, byte p3)
    {
        if ((nint)_netnode_check == 0) throw new System.EntryPointNotFoundException("'netnode_check' is not available for the loaded IDA SDK version.");
        return _netnode_check(p0, p1, p2, p3);
    }

    public static nuint @netnode_copy(ulong p0, ulong p1, ulong p2, byte p3)
    {
        if ((nint)_netnode_copy == 0) throw new System.EntryPointNotFoundException("'netnode_copy' is not available for the loaded IDA SDK version.");
        return _netnode_copy(p0, p1, p2, p3);
    }

    public static int @netnode_delblob(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_delblob == 0) throw new System.EntryPointNotFoundException("'netnode_delblob' is not available for the loaded IDA SDK version.");
        return _netnode_delblob(p0, p1, p2);
    }

    public static byte @netnode_delvalue(ulong p0)
    {
        if ((nint)_netnode_delvalue == 0) throw new System.EntryPointNotFoundException("'netnode_delvalue' is not available for the loaded IDA SDK version.");
        return _netnode_delvalue(p0);
    }

    public static byte @netnode_end(void* p0)
    {
        if ((nint)_netnode_end == 0) throw new System.EntryPointNotFoundException("'netnode_end' is not available for the loaded IDA SDK version.");
        return _netnode_end(p0);
    }

    public static byte @netnode_exist(void* p0)
    {
        if ((nint)_netnode_exist == 0) throw new System.EntryPointNotFoundException("'netnode_exist' is not available for the loaded IDA SDK version.");
        return _netnode_exist(p0);
    }

    public static nint @netnode_get_name(ulong p0, QString* p1)
    {
        if ((nint)_netnode_get_name == 0) throw new System.EntryPointNotFoundException("'netnode_get_name' is not available for the loaded IDA SDK version.");
        return _netnode_get_name(p0, p1);
    }

    public static void* @netnode_getblob(ulong p0, void* p1, nuint* p2, ulong p3, int p4)
    {
        if ((nint)_netnode_getblob == 0) throw new System.EntryPointNotFoundException("'netnode_getblob' is not available for the loaded IDA SDK version.");
        return _netnode_getblob(p0, p1, p2, p3, p4);
    }

    public static byte @netnode_hashdel(ulong p0, byte* p1, int p2)
    {
        if ((nint)_netnode_hashdel == 0) throw new System.EntryPointNotFoundException("'netnode_hashdel' is not available for the loaded IDA SDK version.");
        return _netnode_hashdel(p0, p1, p2);
    }

    public static nint @netnode_hashfirst(ulong p0, byte* p1, nuint p2, int p3)
    {
        if ((nint)_netnode_hashfirst == 0) throw new System.EntryPointNotFoundException("'netnode_hashfirst' is not available for the loaded IDA SDK version.");
        return _netnode_hashfirst(p0, p1, p2, p3);
    }

    public static nint @netnode_hashlast(ulong p0, byte* p1, nuint p2, int p3)
    {
        if ((nint)_netnode_hashlast == 0) throw new System.EntryPointNotFoundException("'netnode_hashlast' is not available for the loaded IDA SDK version.");
        return _netnode_hashlast(p0, p1, p2, p3);
    }

    public static nint @netnode_hashnext(ulong p0, byte* p1, byte* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_hashnext == 0) throw new System.EntryPointNotFoundException("'netnode_hashnext' is not available for the loaded IDA SDK version.");
        return _netnode_hashnext(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_hashprev(ulong p0, byte* p1, byte* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_hashprev == 0) throw new System.EntryPointNotFoundException("'netnode_hashprev' is not available for the loaded IDA SDK version.");
        return _netnode_hashprev(p0, p1, p2, p3, p4);
    }

    public static byte @netnode_hashset(ulong p0, byte* p1, void* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_hashset == 0) throw new System.EntryPointNotFoundException("'netnode_hashset' is not available for the loaded IDA SDK version.");
        return _netnode_hashset(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_hashstr(ulong p0, byte* p1, byte* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_hashstr == 0) throw new System.EntryPointNotFoundException("'netnode_hashstr' is not available for the loaded IDA SDK version.");
        return _netnode_hashstr(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_hashval(ulong p0, byte* p1, void* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_hashval == 0) throw new System.EntryPointNotFoundException("'netnode_hashval' is not available for the loaded IDA SDK version.");
        return _netnode_hashval(p0, p1, p2, p3, p4);
    }

    public static ulong @netnode_hashval_long(ulong p0, byte* p1, int p2)
    {
        if ((nint)_netnode_hashval_long == 0) throw new System.EntryPointNotFoundException("'netnode_hashval_long' is not available for the loaded IDA SDK version.");
        return _netnode_hashval_long(p0, p1, p2);
    }

    public static byte @netnode_inited()
    {
        if ((nint)_netnode_inited == 0) throw new System.EntryPointNotFoundException("'netnode_inited' is not available for the loaded IDA SDK version.");
        return _netnode_inited();
    }

    public static byte @netnode_is_available()
    {
        if ((nint)_netnode_is_available == 0) throw new System.EntryPointNotFoundException("'netnode_is_available' is not available for the loaded IDA SDK version.");
        return _netnode_is_available();
    }

    public static void @netnode_kill(void* p0)
    {
        if ((nint)_netnode_kill == 0) throw new System.EntryPointNotFoundException("'netnode_kill' is not available for the loaded IDA SDK version.");
        _netnode_kill(p0);
    }

    public static ulong @netnode_lower_bound(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_lower_bound == 0) throw new System.EntryPointNotFoundException("'netnode_lower_bound' is not available for the loaded IDA SDK version.");
        return _netnode_lower_bound(p0, p1, p2);
    }

    public static ulong @netnode_lower_bound_idx8(ulong p0, byte p1, int p2)
    {
        if ((nint)_netnode_lower_bound_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_lower_bound_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_lower_bound_idx8(p0, p1, p2);
    }

    public static byte @netnode_next(void* p0)
    {
        if ((nint)_netnode_next == 0) throw new System.EntryPointNotFoundException("'netnode_next' is not available for the loaded IDA SDK version.");
        return _netnode_next(p0);
    }

    public static byte @netnode_prev(void* p0)
    {
        if ((nint)_netnode_prev == 0) throw new System.EntryPointNotFoundException("'netnode_prev' is not available for the loaded IDA SDK version.");
        return _netnode_prev(p0);
    }

    public static nint @netnode_qgetblob(ulong p0, void* p1, nuint p2, ulong p3, int p4)
    {
        if ((nint)_netnode_qgetblob == 0) throw new System.EntryPointNotFoundException("'netnode_qgetblob' is not available for the loaded IDA SDK version.");
        return _netnode_qgetblob(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_qhashfirst(ulong p0, QString* p1, int p2)
    {
        if ((nint)_netnode_qhashfirst == 0) throw new System.EntryPointNotFoundException("'netnode_qhashfirst' is not available for the loaded IDA SDK version.");
        return _netnode_qhashfirst(p0, p1, p2);
    }

    public static nint @netnode_qhashlast(ulong p0, QString* p1, int p2)
    {
        if ((nint)_netnode_qhashlast == 0) throw new System.EntryPointNotFoundException("'netnode_qhashlast' is not available for the loaded IDA SDK version.");
        return _netnode_qhashlast(p0, p1, p2);
    }

    public static nint @netnode_qhashnext(ulong p0, QString* p1, byte* p2, int p3)
    {
        if ((nint)_netnode_qhashnext == 0) throw new System.EntryPointNotFoundException("'netnode_qhashnext' is not available for the loaded IDA SDK version.");
        return _netnode_qhashnext(p0, p1, p2, p3);
    }

    public static nint @netnode_qhashprev(ulong p0, QString* p1, byte* p2, int p3)
    {
        if ((nint)_netnode_qhashprev == 0) throw new System.EntryPointNotFoundException("'netnode_qhashprev' is not available for the loaded IDA SDK version.");
        return _netnode_qhashprev(p0, p1, p2, p3);
    }

    public static nint @netnode_qhashstr(ulong p0, QString* p1, byte* p2, int p3)
    {
        if ((nint)_netnode_qhashstr == 0) throw new System.EntryPointNotFoundException("'netnode_qhashstr' is not available for the loaded IDA SDK version.");
        return _netnode_qhashstr(p0, p1, p2, p3);
    }

    public static nint @netnode_qsupstr(ulong p0, QString* p1, ulong p2, int p3)
    {
        if ((nint)_netnode_qsupstr == 0) throw new System.EntryPointNotFoundException("'netnode_qsupstr' is not available for the loaded IDA SDK version.");
        return _netnode_qsupstr(p0, p1, p2, p3);
    }

    public static nint @netnode_qsupstr_idx8(ulong p0, QString* p1, byte p2, int p3)
    {
        if ((nint)_netnode_qsupstr_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_qsupstr_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_qsupstr_idx8(p0, p1, p2, p3);
    }

    public static nint @netnode_qvalstr(ulong p0, QString* p1)
    {
        if ((nint)_netnode_qvalstr == 0) throw new System.EntryPointNotFoundException("'netnode_qvalstr' is not available for the loaded IDA SDK version.");
        return _netnode_qvalstr(p0, p1);
    }

    public static byte @netnode_rename(ulong p0, byte* p1, nuint p2)
    {
        if ((nint)_netnode_rename == 0) throw new System.EntryPointNotFoundException("'netnode_rename' is not available for the loaded IDA SDK version.");
        return _netnode_rename(p0, p1, p2);
    }

    public static byte @netnode_set(ulong p0, void* p1, nuint p2)
    {
        if ((nint)_netnode_set == 0) throw new System.EntryPointNotFoundException("'netnode_set' is not available for the loaded IDA SDK version.");
        return _netnode_set(p0, p1, p2);
    }

    public static byte @netnode_setblob(ulong p0, void* p1, nuint p2, ulong p3, int p4)
    {
        if ((nint)_netnode_setblob == 0) throw new System.EntryPointNotFoundException("'netnode_setblob' is not available for the loaded IDA SDK version.");
        return _netnode_setblob(p0, p1, p2, p3, p4);
    }

    public static byte @netnode_start(void* p0)
    {
        if ((nint)_netnode_start == 0) throw new System.EntryPointNotFoundException("'netnode_start' is not available for the loaded IDA SDK version.");
        return _netnode_start(p0);
    }

    public static byte @netnode_supdel(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_supdel == 0) throw new System.EntryPointNotFoundException("'netnode_supdel' is not available for the loaded IDA SDK version.");
        return _netnode_supdel(p0, p1, p2);
    }

    public static byte @netnode_supdel_all(ulong p0, int p1)
    {
        if ((nint)_netnode_supdel_all == 0) throw new System.EntryPointNotFoundException("'netnode_supdel_all' is not available for the loaded IDA SDK version.");
        return _netnode_supdel_all(p0, p1);
    }

    public static byte @netnode_supdel_idx8(ulong p0, byte p1, int p2)
    {
        if ((nint)_netnode_supdel_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supdel_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supdel_idx8(p0, p1, p2);
    }

    public static int @netnode_supdel_range(ulong p0, ulong p1, ulong p2, int p3)
    {
        if ((nint)_netnode_supdel_range == 0) throw new System.EntryPointNotFoundException("'netnode_supdel_range' is not available for the loaded IDA SDK version.");
        return _netnode_supdel_range(p0, p1, p2, p3);
    }

    public static int @netnode_supdel_range_idx8(ulong p0, ulong p1, ulong p2, int p3)
    {
        if ((nint)_netnode_supdel_range_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supdel_range_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supdel_range_idx8(p0, p1, p2, p3);
    }

    public static ulong @netnode_supfirst(ulong p0, int p1)
    {
        if ((nint)_netnode_supfirst == 0) throw new System.EntryPointNotFoundException("'netnode_supfirst' is not available for the loaded IDA SDK version.");
        return _netnode_supfirst(p0, p1);
    }

    public static ulong @netnode_supfirst_idx8(ulong p0, int p1)
    {
        if ((nint)_netnode_supfirst_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supfirst_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supfirst_idx8(p0, p1);
    }

    public static ulong @netnode_suplast(ulong p0, int p1)
    {
        if ((nint)_netnode_suplast == 0) throw new System.EntryPointNotFoundException("'netnode_suplast' is not available for the loaded IDA SDK version.");
        return _netnode_suplast(p0, p1);
    }

    public static ulong @netnode_suplast_idx8(ulong p0, int p1)
    {
        if ((nint)_netnode_suplast_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_suplast_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_suplast_idx8(p0, p1);
    }

    public static ulong @netnode_supnext(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_supnext == 0) throw new System.EntryPointNotFoundException("'netnode_supnext' is not available for the loaded IDA SDK version.");
        return _netnode_supnext(p0, p1, p2);
    }

    public static ulong @netnode_supnext_idx8(ulong p0, byte p1, int p2)
    {
        if ((nint)_netnode_supnext_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supnext_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supnext_idx8(p0, p1, p2);
    }

    public static ulong @netnode_supprev(ulong p0, ulong p1, int p2)
    {
        if ((nint)_netnode_supprev == 0) throw new System.EntryPointNotFoundException("'netnode_supprev' is not available for the loaded IDA SDK version.");
        return _netnode_supprev(p0, p1, p2);
    }

    public static ulong @netnode_supprev_idx8(ulong p0, byte p1, int p2)
    {
        if ((nint)_netnode_supprev_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supprev_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supprev_idx8(p0, p1, p2);
    }

    public static byte @netnode_supset(ulong p0, ulong p1, void* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_supset == 0) throw new System.EntryPointNotFoundException("'netnode_supset' is not available for the loaded IDA SDK version.");
        return _netnode_supset(p0, p1, p2, p3, p4);
    }

    public static byte @netnode_supset_idx8(ulong p0, byte p1, void* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_supset_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supset_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supset_idx8(p0, p1, p2, p3, p4);
    }

    public static nuint @netnode_supshift(ulong p0, ulong p1, ulong p2, ulong p3, int p4)
    {
        if ((nint)_netnode_supshift == 0) throw new System.EntryPointNotFoundException("'netnode_supshift' is not available for the loaded IDA SDK version.");
        return _netnode_supshift(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_supstr(ulong p0, ulong p1, byte* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_supstr == 0) throw new System.EntryPointNotFoundException("'netnode_supstr' is not available for the loaded IDA SDK version.");
        return _netnode_supstr(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_supstr_idx8(ulong p0, byte p1, byte* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_supstr_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supstr_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supstr_idx8(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_supval(ulong p0, ulong p1, void* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_supval == 0) throw new System.EntryPointNotFoundException("'netnode_supval' is not available for the loaded IDA SDK version.");
        return _netnode_supval(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_supval_idx8(ulong p0, byte p1, void* p2, nuint p3, int p4)
    {
        if ((nint)_netnode_supval_idx8 == 0) throw new System.EntryPointNotFoundException("'netnode_supval_idx8' is not available for the loaded IDA SDK version.");
        return _netnode_supval_idx8(p0, p1, p2, p3, p4);
    }

    public static nint @netnode_valobj(ulong p0, void* p1, nuint p2)
    {
        if ((nint)_netnode_valobj == 0) throw new System.EntryPointNotFoundException("'netnode_valobj' is not available for the loaded IDA SDK version.");
        return _netnode_valobj(p0, p1, p2);
    }

    public static nint @netnode_valstr(ulong p0, byte* p1, nuint p2)
    {
        if ((nint)_netnode_valstr == 0) throw new System.EntryPointNotFoundException("'netnode_valstr' is not available for the loaded IDA SDK version.");
        return _netnode_valstr(p0, p1, p2);
    }

    public static void* @new_packet(byte p0, byte* p1, nuint p2, int p3)
    {
        if ((nint)_new_packet == 0) throw new System.EntryPointNotFoundException("'new_packet' is not available for the loaded IDA SDK version.");
        return _new_packet(p0, p1, p2, p3);
    }

    public static void* @new_til(byte* p0, byte* p1)
    {
        if ((nint)_new_til == 0) throw new System.EntryPointNotFoundException("'new_til' is not available for the loaded IDA SDK version.");
        return _new_til(p0, p1);
    }

    public static ulong @next_addr(ulong p0)
    {
        if ((nint)_next_addr == 0) throw new System.EntryPointNotFoundException("'next_addr' is not available for the loaded IDA SDK version.");
        return _next_addr(p0);
    }

    public static ulong @next_chunk(ulong p0)
    {
        if ((nint)_next_chunk == 0) throw new System.EntryPointNotFoundException("'next_chunk' is not available for the loaded IDA SDK version.");
        return _next_chunk(p0);
    }

    public static ulong @next_head(ulong p0, ulong p1)
    {
        if ((nint)_next_head == 0) throw new System.EntryPointNotFoundException("'next_head' is not available for the loaded IDA SDK version.");
        return _next_head(p0, p1);
    }

    public static byte* @next_idcv_attr(void* p0, byte* p1)
    {
        if ((nint)_next_idcv_attr == 0) throw new System.EntryPointNotFoundException("'next_idcv_attr' is not available for the loaded IDA SDK version.");
        return _next_idcv_attr(p0, p1);
    }

    public static byte* @next_named_type(void* p0, byte* p1, int p2)
    {
        if ((nint)_next_named_type == 0) throw new System.EntryPointNotFoundException("'next_named_type' is not available for the loaded IDA SDK version.");
        return _next_named_type(p0, p1, p2);
    }

    public static ulong @next_not_tail(ulong p0)
    {
        if ((nint)_next_not_tail == 0) throw new System.EntryPointNotFoundException("'next_not_tail' is not available for the loaded IDA SDK version.");
        return _next_not_tail(p0);
    }

    public static ulong @next_that(ulong p0, ulong p1, void* p2, void* p3)
    {
        if ((nint)_next_that == 0) throw new System.EntryPointNotFoundException("'next_that' is not available for the loaded IDA SDK version.");
        return _next_that(p0, p1, p2, p3);
    }

    public static ulong @next_visea(ulong p0)
    {
        if ((nint)_next_visea == 0) throw new System.EntryPointNotFoundException("'next_visea' is not available for the loaded IDA SDK version.");
        return _next_visea(p0);
    }

    public static ulong @node2ea(ulong p0)
    {
        if ((nint)_node2ea == 0) throw new System.EntryPointNotFoundException("'node2ea' is not available for the loaded IDA SDK version.");
        return _node2ea(p0);
    }

    public static void* @node_iterator_goup(void* p0)
    {
        if ((nint)_node_iterator_goup == 0) throw new System.EntryPointNotFoundException("'node_iterator_goup' is not available for the loaded IDA SDK version.");
        return _node_iterator_goup(p0);
    }

    public static void @notify_dirtree(void* p0, byte p1, ulong p2)
    {
        if ((nint)_notify_dirtree == 0) throw new System.EntryPointNotFoundException("'notify_dirtree' is not available for the loaded IDA SDK version.");
        _notify_dirtree(p0, p1, p2);
    }

    public static ulong @num_flag()
    {
        if ((nint)_num_flag == 0) throw new System.EntryPointNotFoundException("'num_flag' is not available for the loaded IDA SDK version.");
        return _num_flag();
    }

    public static nuint @numop2str(byte* p0, nuint p1, ulong p2, int p3, ulong p4, int p5, int p6)
    {
        if ((nint)_numop2str == 0) throw new System.EntryPointNotFoundException("'numop2str' is not available for the loaded IDA SDK version.");
        return _numop2str(p0, p1, p2, p3, p4, p5, p6);
    }

    public static byte @op_adds_xrefs(ulong p0, int p1)
    {
        if ((nint)_op_adds_xrefs == 0) throw new System.EntryPointNotFoundException("'op_adds_xrefs' is not available for the loaded IDA SDK version.");
        return _op_adds_xrefs(p0, p1);
    }

    public static byte @op_based_stroff(void* p0, int p1, long p2, ulong p3)
    {
        if ((nint)_op_based_stroff == 0) throw new System.EntryPointNotFoundException("'op_based_stroff' is not available for the loaded IDA SDK version.");
        return _op_based_stroff(p0, p1, p2, p3);
    }

    public static byte @op_custfmt(ulong p0, int p1, int p2)
    {
        if ((nint)_op_custfmt == 0) throw new System.EntryPointNotFoundException("'op_custfmt' is not available for the loaded IDA SDK version.");
        return _op_custfmt(p0, p1, p2);
    }

    public static byte @op_enum(ulong p0, int p1, ulong p2, byte p3)
    {
        if ((nint)_op_enum == 0) throw new System.EntryPointNotFoundException("'op_enum' is not available for the loaded IDA SDK version.");
        return _op_enum(p0, p1, p2, p3);
    }

    public static byte @op_offset(ulong p0, int p1, uint p2, ulong p3, ulong p4, long p5)
    {
        if ((nint)_op_offset == 0) throw new System.EntryPointNotFoundException("'op_offset' is not available for the loaded IDA SDK version.");
        return _op_offset(p0, p1, p2, p3, p4, p5);
    }

    public static byte @op_offset_ex(ulong p0, int p1, void* p2)
    {
        if ((nint)_op_offset_ex == 0) throw new System.EntryPointNotFoundException("'op_offset_ex' is not available for the loaded IDA SDK version.");
        return _op_offset_ex(p0, p1, p2);
    }

    public static byte @op_seg(ulong p0, int p1)
    {
        if ((nint)_op_seg == 0) throw new System.EntryPointNotFoundException("'op_seg' is not available for the loaded IDA SDK version.");
        return _op_seg(p0, p1);
    }

    public static byte @op_stkvar(ulong p0, int p1)
    {
        if ((nint)_op_stkvar == 0) throw new System.EntryPointNotFoundException("'op_stkvar' is not available for the loaded IDA SDK version.");
        return _op_stkvar(p0, p1);
    }

    public static byte @op_stroff(void* p0, int p1, ulong* p2, int p3, long p4)
    {
        if ((nint)_op_stroff == 0) throw new System.EntryPointNotFoundException("'op_stroff' is not available for the loaded IDA SDK version.");
        return _op_stroff(p0, p1, p2, p3, p4);
    }

    public static void* @openM(byte* p0)
    {
        if ((nint)_openM == 0) throw new System.EntryPointNotFoundException("'openM' is not available for the loaded IDA SDK version.");
        return _openM(p0);
    }

    public static void* @openR(byte* p0)
    {
        if ((nint)_openR == 0) throw new System.EntryPointNotFoundException("'openR' is not available for the loaded IDA SDK version.");
        return _openR(p0);
    }

    public static void* @openRT(byte* p0)
    {
        if ((nint)_openRT == 0) throw new System.EntryPointNotFoundException("'openRT' is not available for the loaded IDA SDK version.");
        return _openRT(p0);
    }

    public static int @open_database(byte* p0, byte p1, byte* p2)
    {
        if ((nint)_open_database == 0) throw new System.EntryPointNotFoundException("'open_database' is not available for the loaded IDA SDK version.");
        return _open_database(p0, p1, p2);
    }

    public static void* @open_linput(byte* p0, byte p1)
    {
        if ((nint)_open_linput == 0) throw new System.EntryPointNotFoundException("'open_linput' is not available for the loaded IDA SDK version.");
        return _open_linput(p0, p1);
    }

    public static byte @optimize_argloc(void* p0, int p1, void* p2)
    {
        if ((nint)_optimize_argloc == 0) throw new System.EntryPointNotFoundException("'optimize_argloc' is not available for the loaded IDA SDK version.");
        return _optimize_argloc(p0, p1, p2);
    }

    public static byte* @pack_dd(byte* p0, byte* p1, uint p2)
    {
        if ((nint)_pack_dd == 0) throw new System.EntryPointNotFoundException("'pack_dd' is not available for the loaded IDA SDK version.");
        return _pack_dd(p0, p1, p2);
    }

    public static byte* @pack_dq(byte* p0, byte* p1, ulong p2)
    {
        if ((nint)_pack_dq == 0) throw new System.EntryPointNotFoundException("'pack_dq' is not available for the loaded IDA SDK version.");
        return _pack_dq(p0, p1, p2);
    }

    public static byte* @pack_ds(byte* p0, byte* p1, byte* p2, nuint p3)
    {
        if ((nint)_pack_ds == 0) throw new System.EntryPointNotFoundException("'pack_ds' is not available for the loaded IDA SDK version.");
        return _pack_ds(p0, p1, p2, p3);
    }

    public static byte* @pack_dw(byte* p0, byte* p1, ushort p2)
    {
        if ((nint)_pack_dw == 0) throw new System.EntryPointNotFoundException("'pack_dw' is not available for the loaded IDA SDK version.");
        return _pack_dw(p0, p1, p2);
    }

    public static int @pack_idcobj_to_bv(void* p0, TypeInfo* p1, void* p2, void* p3, int p4)
    {
        if ((nint)_pack_idcobj_to_bv == 0) throw new System.EntryPointNotFoundException("'pack_idcobj_to_bv' is not available for the loaded IDA SDK version.");
        return _pack_idcobj_to_bv(p0, p1, p2, p3, p4);
    }

    public static int @pack_idcobj_to_idb(void* p0, TypeInfo* p1, ulong p2, int p3)
    {
        if ((nint)_pack_idcobj_to_idb == 0) throw new System.EntryPointNotFoundException("'pack_idcobj_to_idb' is not available for the loaded IDA SDK version.");
        return _pack_idcobj_to_idb(p0, p1, p2, p3);
    }

    public static byte @parse_binpat_str(void* p0, ulong p1, byte* p2, int p3, int p4, QString* p5)
    {
        if ((nint)_parse_binpat_str == 0) throw new System.EntryPointNotFoundException("'parse_binpat_str' is not available for the loaded IDA SDK version.");
        return _parse_binpat_str(p0, p1, p2, p3, p4, p5);
    }

    public static nuint @parse_command_line(void* p0, void* p1, byte* p2, int p3)
    {
        if ((nint)_parse_command_line == 0) throw new System.EntryPointNotFoundException("'parse_command_line' is not available for the loaded IDA SDK version.");
        return _parse_command_line(p0, p1, p2, p3);
    }

    public static byte @parse_config_value(void* p0, void* p1, void* p2)
    {
        if ((nint)_parse_config_value == 0) throw new System.EntryPointNotFoundException("'parse_config_value' is not available for the loaded IDA SDK version.");
        return _parse_config_value(p0, p1, p2);
    }

    public static byte @parse_dbgopts(void* p0, byte* p1)
    {
        if ((nint)_parse_dbgopts == 0) throw new System.EntryPointNotFoundException("'parse_dbgopts' is not available for the loaded IDA SDK version.");
        return _parse_dbgopts(p0, p1);
    }

    public static byte @parse_decl(TypeInfo* p0, QString* p1, void* p2, byte* p3, int p4)
    {
        if ((nint)_parse_decl == 0) throw new System.EntryPointNotFoundException("'parse_decl' is not available for the loaded IDA SDK version.");
        return _parse_decl(p0, p1, p2, p3, p4);
    }

    public static int @parse_decls(void* p0, byte* p1, void* p2, int p3)
    {
        if ((nint)_parse_decls == 0) throw new System.EntryPointNotFoundException("'parse_decls' is not available for the loaded IDA SDK version.");
        return _parse_decls(p0, p1, p2, p3);
    }

    public static int @parse_decls_for_srclang(int p0, void* p1, byte* p2, byte p3)
    {
        if ((nint)_parse_decls_for_srclang == 0) throw new System.EntryPointNotFoundException("'parse_decls_for_srclang' is not available for the loaded IDA SDK version.");
        return _parse_decls_for_srclang(p0, p1, p2, p3);
    }

    public static int @parse_decls_with_parser(byte* p0, void* p1, byte* p2, byte p3)
    {
        if ((nint)_parse_decls_with_parser == 0) throw new System.EntryPointNotFoundException("'parse_decls_with_parser' is not available for the loaded IDA SDK version.");
        return _parse_decls_with_parser(p0, p1, p2, p3);
    }

    public static int @parse_decls_with_parser_ext(byte* p0, void* p1, byte* p2, int p3)
    {
        if ((nint)_parse_decls_with_parser_ext == 0) throw new System.EntryPointNotFoundException("'parse_decls_with_parser_ext' is not available for the loaded IDA SDK version.");
        return _parse_decls_with_parser_ext(p0, p1, p2, p3);
    }

    public static int @parse_json(void* p0, void* p1, void* p2)
    {
        if ((nint)_parse_json == 0) throw new System.EntryPointNotFoundException("'parse_json' is not available for the loaded IDA SDK version.");
        return _parse_json(p0, p1, p2);
    }

    public static int @parse_json_file(void* p0, byte* p1, QString* p2)
    {
        if ((nint)_parse_json_file == 0) throw new System.EntryPointNotFoundException("'parse_json_file' is not available for the loaded IDA SDK version.");
        return _parse_json_file(p0, p1, p2);
    }

    public static int @parse_json_string(void* p0, byte* p1, QString* p2, byte* p3)
    {
        if ((nint)_parse_json_string == 0) throw new System.EntryPointNotFoundException("'parse_json_string' is not available for the loaded IDA SDK version.");
        return _parse_json_string(p0, p1, p2, p3);
    }

    public static byte @parse_plugin_options(void* p0, byte* p1)
    {
        if ((nint)_parse_plugin_options == 0) throw new System.EntryPointNotFoundException("'parse_plugin_options' is not available for the loaded IDA SDK version.");
        return _parse_plugin_options(p0, p1);
    }

    public static byte @parse_pretty_size(ulong* p0, byte* p1)
    {
        if ((nint)_parse_pretty_size == 0) throw new System.EntryPointNotFoundException("'parse_pretty_size' is not available for the loaded IDA SDK version.");
        return _parse_pretty_size(p0, p1);
    }

    public static byte @parse_reg_name(void* p0, byte* p1)
    {
        if ((nint)_parse_reg_name == 0) throw new System.EntryPointNotFoundException("'parse_reg_name' is not available for the loaded IDA SDK version.");
        return _parse_reg_name(p0, p1);
    }

    public static byte @parse_timestamp(ulong* p0, byte* p1, uint p2)
    {
        if ((nint)_parse_timestamp == 0) throw new System.EntryPointNotFoundException("'parse_timestamp' is not available for the loaded IDA SDK version.");
        return _parse_timestamp(p0, p1, p2);
    }

    public static byte @patch_byte(ulong p0, ulong p1)
    {
        if ((nint)_patch_byte == 0) throw new System.EntryPointNotFoundException("'patch_byte' is not available for the loaded IDA SDK version.");
        return _patch_byte(p0, p1);
    }

    public static void @patch_bytes(ulong p0, void* p1, nuint p2)
    {
        if ((nint)_patch_bytes == 0) throw new System.EntryPointNotFoundException("'patch_bytes' is not available for the loaded IDA SDK version.");
        _patch_bytes(p0, p1, p2);
    }

    public static byte @patch_dword(ulong p0, ulong p1)
    {
        if ((nint)_patch_dword == 0) throw new System.EntryPointNotFoundException("'patch_dword' is not available for the loaded IDA SDK version.");
        return _patch_dword(p0, p1);
    }

    public static byte @patch_fixup_value(ulong p0, void* p1)
    {
        if ((nint)_patch_fixup_value == 0) throw new System.EntryPointNotFoundException("'patch_fixup_value' is not available for the loaded IDA SDK version.");
        return _patch_fixup_value(p0, p1);
    }

    public static byte @patch_qword(ulong p0, ulong p1)
    {
        if ((nint)_patch_qword == 0) throw new System.EntryPointNotFoundException("'patch_qword' is not available for the loaded IDA SDK version.");
        return _patch_qword(p0, p1);
    }

    public static byte @patch_word(ulong p0, ulong p1)
    {
        if ((nint)_patch_word == 0) throw new System.EntryPointNotFoundException("'patch_word' is not available for the loaded IDA SDK version.");
        return _patch_word(p0, p1);
    }

    public static ulong @peek_auto_queue(ulong p0, int p1)
    {
        if ((nint)_peek_auto_queue == 0) throw new System.EntryPointNotFoundException("'peek_auto_queue' is not available for the loaded IDA SDK version.");
        return _peek_auto_queue(p0, p1);
    }

    public static byte @perform_redo()
    {
        if ((nint)_perform_redo == 0) throw new System.EntryPointNotFoundException("'perform_redo' is not available for the loaded IDA SDK version.");
        return _perform_redo();
    }

    public static byte @perform_undo()
    {
        if ((nint)_perform_undo == 0) throw new System.EntryPointNotFoundException("'perform_undo' is not available for the loaded IDA SDK version.");
        return _perform_undo();
    }

    public static void* @pipe_process(int* p0, int* p1, void* p2, QString* p3)
    {
        if ((nint)_pipe_process == 0) throw new System.EntryPointNotFoundException("'pipe_process' is not available for the loaded IDA SDK version.");
        return _pipe_process(p0, p1, p2, p3);
    }

    public static int @plan_and_wait(ulong p0, ulong p1, byte p2)
    {
        if ((nint)_plan_and_wait == 0) throw new System.EntryPointNotFoundException("'plan_and_wait' is not available for the loaded IDA SDK version.");
        return _plan_and_wait(p0, p1, p2);
    }

    public static int @plan_to_apply_idasgn(byte* p0)
    {
        if ((nint)_plan_to_apply_idasgn == 0) throw new System.EntryPointNotFoundException("'plan_to_apply_idasgn' is not available for the loaded IDA SDK version.");
        return _plan_to_apply_idasgn(p0);
    }

    public static void @plugin_name_from_path_or_name(QString* p0, byte* p1)
    {
        if ((nint)_plugin_name_from_path_or_name == 0) throw new System.EntryPointNotFoundException("'plugin_name_from_path_or_name' is not available for the loaded IDA SDK version.");
        _plugin_name_from_path_or_name(p0, p1);
    }

    public static byte @plugin_option_t_get_bool(void* p0, byte* p1, byte* p2, byte p3)
    {
        if ((nint)_plugin_option_t_get_bool == 0) throw new System.EntryPointNotFoundException("'plugin_option_t_get_bool' is not available for the loaded IDA SDK version.");
        return _plugin_option_t_get_bool(p0, p1, p2, p3);
    }

    public static nuint @pretty_print_size(byte* p0, nuint p1, ulong p2)
    {
        if ((nint)_pretty_print_size == 0) throw new System.EntryPointNotFoundException("'pretty_print_size' is not available for the loaded IDA SDK version.");
        return _pretty_print_size(p0, p1, p2);
    }

    public static ulong @prev_addr(ulong p0)
    {
        if ((nint)_prev_addr == 0) throw new System.EntryPointNotFoundException("'prev_addr' is not available for the loaded IDA SDK version.");
        return _prev_addr(p0);
    }

    public static ulong @prev_chunk(ulong p0)
    {
        if ((nint)_prev_chunk == 0) throw new System.EntryPointNotFoundException("'prev_chunk' is not available for the loaded IDA SDK version.");
        return _prev_chunk(p0);
    }

    public static ulong @prev_head(ulong p0, ulong p1)
    {
        if ((nint)_prev_head == 0) throw new System.EntryPointNotFoundException("'prev_head' is not available for the loaded IDA SDK version.");
        return _prev_head(p0, p1);
    }

    public static byte* @prev_idcv_attr(void* p0, byte* p1)
    {
        if ((nint)_prev_idcv_attr == 0) throw new System.EntryPointNotFoundException("'prev_idcv_attr' is not available for the loaded IDA SDK version.");
        return _prev_idcv_attr(p0, p1);
    }

    public static ulong @prev_not_tail(ulong p0)
    {
        if ((nint)_prev_not_tail == 0) throw new System.EntryPointNotFoundException("'prev_not_tail' is not available for the loaded IDA SDK version.");
        return _prev_not_tail(p0);
    }

    public static ulong @prev_that(ulong p0, ulong p1, void* p2, void* p3)
    {
        if ((nint)_prev_that == 0) throw new System.EntryPointNotFoundException("'prev_that' is not available for the loaded IDA SDK version.");
        return _prev_that(p0, p1, p2, p3);
    }

    public static byte @prev_utf8_char(uint* p0, byte** p1, byte* p2)
    {
        if ((nint)_prev_utf8_char == 0) throw new System.EntryPointNotFoundException("'prev_utf8_char' is not available for the loaded IDA SDK version.");
        return _prev_utf8_char(p0, p1, p2);
    }

    public static ulong @prev_visea(ulong p0)
    {
        if ((nint)_prev_visea == 0) throw new System.EntryPointNotFoundException("'prev_visea' is not available for the loaded IDA SDK version.");
        return _prev_visea(p0);
    }

    public static nuint @print_argloc(byte* p0, nuint p1, void* p2, int p3, int p4)
    {
        if ((nint)_print_argloc == 0) throw new System.EntryPointNotFoundException("'print_argloc' is not available for the loaded IDA SDK version.");
        return _print_argloc(p0, p1, p2, p3, p4);
    }

    public static int @print_cdata(void* p0, void* p1, TypeInfo* p2, void* p3)
    {
        if ((nint)_print_cdata == 0) throw new System.EntryPointNotFoundException("'print_cdata' is not available for the loaded IDA SDK version.");
        return _print_cdata(p0, p1, p2, p3);
    }

    public static int @print_decls(void* p0, void* p1, void* p2, uint p3)
    {
        if ((nint)_print_decls == 0) throw new System.EntryPointNotFoundException("'print_decls' is not available for the loaded IDA SDK version.");
        return _print_decls(p0, p1, p2, p3);
    }

    public static byte @print_fpval(byte* p0, nuint p1, void* p2, int p3)
    {
        if ((nint)_print_fpval == 0) throw new System.EntryPointNotFoundException("'print_fpval' is not available for the loaded IDA SDK version.");
        return _print_fpval(p0, p1, p2, p3);
    }

    public static byte @print_idcv(QString* p0, void* p1, byte* p2, int p3)
    {
        if ((nint)_print_idcv == 0) throw new System.EntryPointNotFoundException("'print_idcv' is not available for the loaded IDA SDK version.");
        return _print_idcv(p0, p1, p2, p3);
    }

    public static byte @print_insn_mnem(QString* p0, ulong p1)
    {
        if ((nint)_print_insn_mnem == 0) throw new System.EntryPointNotFoundException("'print_insn_mnem' is not available for the loaded IDA SDK version.");
        return _print_insn_mnem(p0, p1);
    }

    public static byte @print_operand(QString* p0, ulong p1, int p2, int p3, void* p4)
    {
        if ((nint)_print_operand == 0) throw new System.EntryPointNotFoundException("'print_operand' is not available for the loaded IDA SDK version.");
        return _print_operand(p0, p1, p2, p3, p4);
    }

    public static byte @print_strlit_type(QString* p0, int p1, QString* p2, int p3)
    {
        if ((nint)_print_strlit_type == 0) throw new System.EntryPointNotFoundException("'print_strlit_type' is not available for the loaded IDA SDK version.");
        return _print_strlit_type(p0, p1, p2, p3);
    }

    public static byte @print_tinfo(QString* p0, byte* p1, int p2, int p3, int p4, TypeInfo* p5, byte* p6, byte* p7)
    {
        if ((nint)_print_tinfo == 0) throw new System.EntryPointNotFoundException("'print_tinfo' is not available for the loaded IDA SDK version.");
        return _print_tinfo(p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static byte @print_type(QString* p0, ulong p1, int p2)
    {
        if ((nint)_print_type == 0) throw new System.EntryPointNotFoundException("'print_type' is not available for the loaded IDA SDK version.");
        return _print_type(p0, p1, p2);
    }

    public static int @process_archive(QString* p0, void* p1, QString* p2, ushort* p3, byte* p4, void* p5, QString* p6)
    {
        if ((nint)_process_archive == 0) throw new System.EntryPointNotFoundException("'process_archive' is not available for the loaded IDA SDK version.");
        return _process_archive(p0, p1, p2, p3, p4, p5, p6);
    }

    public static void @process_config_directive(byte* p0, int p1)
    {
        if ((nint)_process_config_directive == 0) throw new System.EntryPointNotFoundException("'process_config_directive' is not available for the loaded IDA SDK version.");
        _process_config_directive(p0, p1);
    }

    public static byte @put_byte(ulong p0, ulong p1)
    {
        if ((nint)_put_byte == 0) throw new System.EntryPointNotFoundException("'put_byte' is not available for the loaded IDA SDK version.");
        return _put_byte(p0, p1);
    }

    public static void @put_bytes(ulong p0, void* p1, nuint p2)
    {
        if ((nint)_put_bytes == 0) throw new System.EntryPointNotFoundException("'put_bytes' is not available for the loaded IDA SDK version.");
        _put_bytes(p0, p1, p2);
    }

    public static byte @put_dbg_byte(ulong p0, uint p1)
    {
        if ((nint)_put_dbg_byte == 0) throw new System.EntryPointNotFoundException("'put_dbg_byte' is not available for the loaded IDA SDK version.");
        return _put_dbg_byte(p0, p1);
    }

    public static void @put_dword(ulong p0, ulong p1)
    {
        if ((nint)_put_dword == 0) throw new System.EntryPointNotFoundException("'put_dword' is not available for the loaded IDA SDK version.");
        _put_dword(p0, p1);
    }

    public static void @put_qword(ulong p0, ulong p1)
    {
        if ((nint)_put_qword == 0) throw new System.EntryPointNotFoundException("'put_qword' is not available for the loaded IDA SDK version.");
        _put_qword(p0, p1);
    }

    public static nint @put_utf8_char(byte* p0, uint p1)
    {
        if ((nint)_put_utf8_char == 0) throw new System.EntryPointNotFoundException("'put_utf8_char' is not available for the loaded IDA SDK version.");
        return _put_utf8_char(p0, p1);
    }

    public static void @put_word(ulong p0, ulong p1)
    {
        if ((nint)_put_word == 0) throw new System.EntryPointNotFoundException("'put_word' is not available for the loaded IDA SDK version.");
        _put_word(p0, p1);
    }

    public static int @qaccess(byte* p0, int p1)
    {
        if ((nint)_qaccess == 0) throw new System.EntryPointNotFoundException("'qaccess' is not available for the loaded IDA SDK version.");
        return _qaccess(p0, p1);
    }

    public static void* @qalloc(nuint p0)
    {
        if ((nint)_qalloc == 0) throw new System.EntryPointNotFoundException("'qalloc' is not available for the loaded IDA SDK version.");
        return _qalloc(p0);
    }

    public static void* @qalloc_or_throw(nuint p0)
    {
        if ((nint)_qalloc_or_throw == 0) throw new System.EntryPointNotFoundException("'qalloc_or_throw' is not available for the loaded IDA SDK version.");
        return _qalloc_or_throw(p0);
    }

    public static byte* @qbasename(byte* p0)
    {
        if ((nint)_qbasename == 0) throw new System.EntryPointNotFoundException("'qbasename' is not available for the loaded IDA SDK version.");
        return _qbasename(p0);
    }

    public static void* @qcalloc(nuint p0, nuint p1)
    {
        if ((nint)_qcalloc == 0) throw new System.EntryPointNotFoundException("'qcalloc' is not available for the loaded IDA SDK version.");
        return _qcalloc(p0, p1);
    }

    public static int @qchdir(byte* p0)
    {
        if ((nint)_qchdir == 0) throw new System.EntryPointNotFoundException("'qchdir' is not available for the loaded IDA SDK version.");
        return _qchdir(p0);
    }

    public static int @qchsize(int p0, ulong p1)
    {
        if ((nint)_qchsize == 0) throw new System.EntryPointNotFoundException("'qchsize' is not available for the loaded IDA SDK version.");
        return _qchsize(p0, p1);
    }

    public static nint @qcleanline(QString* p0, byte p1, uint p2)
    {
        if ((nint)_qcleanline == 0) throw new System.EntryPointNotFoundException("'qcleanline' is not available for the loaded IDA SDK version.");
        return _qcleanline(p0, p1, p2);
    }

    public static int @qclose(int p0)
    {
        if ((nint)_qclose == 0) throw new System.EntryPointNotFoundException("'qclose' is not available for the loaded IDA SDK version.");
        return _qclose(p0);
    }

    public static void @qcontrol_tty()
    {
        if ((nint)_qcontrol_tty == 0) throw new System.EntryPointNotFoundException("'qcontrol_tty' is not available for the loaded IDA SDK version.");
        _qcontrol_tty();
    }

    public static int @qcreate(byte* p0, int p1)
    {
        if ((nint)_qcreate == 0) throw new System.EntryPointNotFoundException("'qcreate' is not available for the loaded IDA SDK version.");
        return _qcreate(p0, p1);
    }

    public static byte @qctime(byte* p0, nuint p1, int p2)
    {
        if ((nint)_qctime == 0) throw new System.EntryPointNotFoundException("'qctime' is not available for the loaded IDA SDK version.");
        return _qctime(p0, p1, p2);
    }

    public static byte @qctime_utc(byte* p0, nuint p1, int p2)
    {
        if ((nint)_qctime_utc == 0) throw new System.EntryPointNotFoundException("'qctime_utc' is not available for the loaded IDA SDK version.");
        return _qctime_utc(p0, p1, p2);
    }

    public static void @qdetach_tty()
    {
        if ((nint)_qdetach_tty == 0) throw new System.EntryPointNotFoundException("'qdetach_tty' is not available for the loaded IDA SDK version.");
        _qdetach_tty();
    }

    public static byte @qdirname(byte* p0, nuint p1, byte* p2)
    {
        if ((nint)_qdirname == 0) throw new System.EntryPointNotFoundException("'qdirname' is not available for the loaded IDA SDK version.");
        return _qdirname(p0, p1, p2);
    }

    public static int @qdup(int p0)
    {
        if ((nint)_qdup == 0) throw new System.EntryPointNotFoundException("'qdup' is not available for the loaded IDA SDK version.");
        return _qdup(p0);
    }

    public static byte* @qerrstr(int p0)
    {
        if ((nint)_qerrstr == 0) throw new System.EntryPointNotFoundException("'qerrstr' is not available for the loaded IDA SDK version.");
        return _qerrstr(p0);
    }

    public static void @qexit(int p0)
    {
        if ((nint)_qexit == 0) throw new System.EntryPointNotFoundException("'qexit' is not available for the loaded IDA SDK version.");
        _qexit(p0);
    }

    public static int @qfclose(void* p0)
    {
        if ((nint)_qfclose == 0) throw new System.EntryPointNotFoundException("'qfclose' is not available for the loaded IDA SDK version.");
        return _qfclose(p0);
    }

    public static int @qfgetc(void* p0)
    {
        if ((nint)_qfgetc == 0) throw new System.EntryPointNotFoundException("'qfgetc' is not available for the loaded IDA SDK version.");
        return _qfgetc(p0);
    }

    public static byte* @qfgets(byte* p0, nuint p1, void* p2)
    {
        if ((nint)_qfgets == 0) throw new System.EntryPointNotFoundException("'qfgets' is not available for the loaded IDA SDK version.");
        return _qfgets(p0, p1, p2);
    }

    public static byte @qfileexist(byte* p0)
    {
        if ((nint)_qfileexist == 0) throw new System.EntryPointNotFoundException("'qfileexist' is not available for the loaded IDA SDK version.");
        return _qfileexist(p0);
    }

    public static ulong @qfilelength(int p0)
    {
        if ((nint)_qfilelength == 0) throw new System.EntryPointNotFoundException("'qfilelength' is not available for the loaded IDA SDK version.");
        return _qfilelength(p0);
    }

    public static ulong @qfilesize(byte* p0)
    {
        if ((nint)_qfilesize == 0) throw new System.EntryPointNotFoundException("'qfilesize' is not available for the loaded IDA SDK version.");
        return _qfilesize(p0);
    }

    public static void @qfindclose(void* p0)
    {
        if ((nint)_qfindclose == 0) throw new System.EntryPointNotFoundException("'qfindclose' is not available for the loaded IDA SDK version.");
        _qfindclose(p0);
    }

    public static int @qfindfirst(byte* p0, void* p1, int p2)
    {
        if ((nint)_qfindfirst == 0) throw new System.EntryPointNotFoundException("'qfindfirst' is not available for the loaded IDA SDK version.");
        return _qfindfirst(p0, p1, p2);
    }

    public static int @qfindnext(void* p0)
    {
        if ((nint)_qfindnext == 0) throw new System.EntryPointNotFoundException("'qfindnext' is not available for the loaded IDA SDK version.");
        return _qfindnext(p0);
    }

    public static int @qflush(void* p0)
    {
        if ((nint)_qflush == 0) throw new System.EntryPointNotFoundException("'qflush' is not available for the loaded IDA SDK version.");
        return _qflush(p0);
    }

    public static void* @qfopen(byte* p0, byte* p1)
    {
        if ((nint)_qfopen == 0) throw new System.EntryPointNotFoundException("'qfopen' is not available for the loaded IDA SDK version.");
        return _qfopen(p0, p1);
    }

    public static int @qfputc(int p0, void* p1)
    {
        if ((nint)_qfputc == 0) throw new System.EntryPointNotFoundException("'qfputc' is not available for the loaded IDA SDK version.");
        return _qfputc(p0, p1);
    }

    public static int @qfputs(byte* p0, void* p1)
    {
        if ((nint)_qfputs == 0) throw new System.EntryPointNotFoundException("'qfputs' is not available for the loaded IDA SDK version.");
        return _qfputs(p0, p1);
    }

    public static nint @qfread(void* p0, void* p1, nuint p2)
    {
        if ((nint)_qfread == 0) throw new System.EntryPointNotFoundException("'qfread' is not available for the loaded IDA SDK version.");
        return _qfread(p0, p1, p2);
    }

    public static void @qfree(void* p0)
    {
        if ((nint)_qfree == 0) throw new System.EntryPointNotFoundException("'qfree' is not available for the loaded IDA SDK version.");
        _qfree(p0);
    }

    public static ulong @qfsize(void* p0)
    {
        if ((nint)_qfsize == 0) throw new System.EntryPointNotFoundException("'qfsize' is not available for the loaded IDA SDK version.");
        return _qfsize(p0);
    }

    public static int @qfstat(int p0, void* p1)
    {
        if ((nint)_qfstat == 0) throw new System.EntryPointNotFoundException("'qfstat' is not available for the loaded IDA SDK version.");
        return _qfstat(p0, p1);
    }

    public static int @qfsync(int p0)
    {
        if ((nint)_qfsync == 0) throw new System.EntryPointNotFoundException("'qfsync' is not available for the loaded IDA SDK version.");
        return _qfsync(p0);
    }

    public static nint @qfwrite(void* p0, void* p1, nuint p2)
    {
        if ((nint)_qfwrite == 0) throw new System.EntryPointNotFoundException("'qfwrite' is not available for the loaded IDA SDK version.");
        return _qfwrite(p0, p1, p2);
    }

    public static void @qgetcwd(byte* p0, nuint p1)
    {
        if ((nint)_qgetcwd == 0) throw new System.EntryPointNotFoundException("'qgetcwd' is not available for the loaded IDA SDK version.");
        _qgetcwd(p0, p1);
    }

    public static byte @qgethostname(QString* p0)
    {
        if ((nint)_qgethostname == 0) throw new System.EntryPointNotFoundException("'qgethostname' is not available for the loaded IDA SDK version.");
        return _qgethostname(p0);
    }

    public static nint @qgetline(QString* p0, void* p1)
    {
        if ((nint)_qgetline == 0) throw new System.EntryPointNotFoundException("'qgetline' is not available for the loaded IDA SDK version.");
        return _qgetline(p0, p1);
    }

    public static byte* @qgets(byte* p0, nuint p1)
    {
        if ((nint)_qgets == 0) throw new System.EntryPointNotFoundException("'qgets' is not available for the loaded IDA SDK version.");
        return _qgets(p0, p1);
    }

    public static byte @qhost2addr_(void* p0, byte* p1, ushort p2, ushort p3)
    {
        if ((nint)_qhost2addr_ == 0) throw new System.EntryPointNotFoundException("'qhost2addr_' is not available for the loaded IDA SDK version.");
        return _qhost2addr_(p0, p1, p2, p3);
    }

    public static byte @qisabspath(byte* p0)
    {
        if ((nint)_qisabspath == 0) throw new System.EntryPointNotFoundException("'qisabspath' is not available for the loaded IDA SDK version.");
        return _qisabspath(p0);
    }

    public static byte @qisdir(byte* p0)
    {
        if ((nint)_qisdir == 0) throw new System.EntryPointNotFoundException("'qisdir' is not available for the loaded IDA SDK version.");
        return _qisdir(p0);
    }

    public static void* @qlfile(void* p0)
    {
        if ((nint)_qlfile == 0) throw new System.EntryPointNotFoundException("'qlfile' is not available for the loaded IDA SDK version.");
        return _qlfile(p0);
    }

    public static int @qlgetc(void* p0)
    {
        if ((nint)_qlgetc == 0) throw new System.EntryPointNotFoundException("'qlgetc' is not available for the loaded IDA SDK version.");
        return _qlgetc(p0);
    }

    public static byte* @qlgets(byte* p0, nuint p1, void* p2)
    {
        if ((nint)_qlgets == 0) throw new System.EntryPointNotFoundException("'qlgets' is not available for the loaded IDA SDK version.");
        return _qlgets(p0, p1, p2);
    }

    public static byte* @qlgetz(void* p0, long p1, byte* p2, nuint p3)
    {
        if ((nint)_qlgetz == 0) throw new System.EntryPointNotFoundException("'qlgetz' is not available for the loaded IDA SDK version.");
        return _qlgetz(p0, p1, p2, p3);
    }

    public static nint @qlread(void* p0, void* p1, nuint p2)
    {
        if ((nint)_qlread == 0) throw new System.EntryPointNotFoundException("'qlread' is not available for the loaded IDA SDK version.");
        return _qlread(p0, p1, p2);
    }

    public static long @qlsize(void* p0)
    {
        if ((nint)_qlsize == 0) throw new System.EntryPointNotFoundException("'qlsize' is not available for the loaded IDA SDK version.");
        return _qlsize(p0);
    }

    public static byte* @qmake_full_path(byte* p0, nuint p1, byte* p2)
    {
        if ((nint)_qmake_full_path == 0) throw new System.EntryPointNotFoundException("'qmake_full_path' is not available for the loaded IDA SDK version.");
        return _qmake_full_path(p0, p1, p2);
    }

    public static byte* @qmakefile(byte* p0, nuint p1, byte* p2, byte* p3)
    {
        if ((nint)_qmakefile == 0) throw new System.EntryPointNotFoundException("'qmakefile' is not available for the loaded IDA SDK version.");
        return _qmakefile(p0, p1, p2, p3);
    }

    public static int @qmkdir(byte* p0, int p1)
    {
        if ((nint)_qmkdir == 0) throw new System.EntryPointNotFoundException("'qmkdir' is not available for the loaded IDA SDK version.");
        return _qmkdir(p0, p1);
    }

    public static int @qmove(byte* p0, byte* p1, uint p2)
    {
        if ((nint)_qmove == 0) throw new System.EntryPointNotFoundException("'qmove' is not available for the loaded IDA SDK version.");
        return _qmove(p0, p1, p2);
    }

    public static int @qopen(byte* p0, int p1)
    {
        if ((nint)_qopen == 0) throw new System.EntryPointNotFoundException("'qopen' is not available for the loaded IDA SDK version.");
        return _qopen(p0, p1);
    }

    public static int @qopen_shared(byte* p0, int p1, int p2)
    {
        if ((nint)_qopen_shared == 0) throw new System.EntryPointNotFoundException("'qopen_shared' is not available for the loaded IDA SDK version.");
        return _qopen_shared(p0, p1, p2);
    }

    public static int @qpipe_close(int p0)
    {
        if ((nint)_qpipe_close == 0) throw new System.EntryPointNotFoundException("'qpipe_close' is not available for the loaded IDA SDK version.");
        return _qpipe_close(p0);
    }

    public static nint @qpipe_read(int p0, void* p1, nuint p2)
    {
        if ((nint)_qpipe_read == 0) throw new System.EntryPointNotFoundException("'qpipe_read' is not available for the loaded IDA SDK version.");
        return _qpipe_read(p0, p1, p2);
    }

    public static byte @qpipe_read_n(int p0, void* p1, nuint p2)
    {
        if ((nint)_qpipe_read_n == 0) throw new System.EntryPointNotFoundException("'qpipe_read_n' is not available for the loaded IDA SDK version.");
        return _qpipe_read_n(p0, p1, p2);
    }

    public static nint @qpipe_write(int p0, void* p1, nuint p2)
    {
        if ((nint)_qpipe_write == 0) throw new System.EntryPointNotFoundException("'qpipe_write' is not available for the loaded IDA SDK version.");
        return _qpipe_write(p0, p1, p2);
    }

    public static int @qread(int p0, void* p1, nuint p2)
    {
        if ((nint)_qread == 0) throw new System.EntryPointNotFoundException("'qread' is not available for the loaded IDA SDK version.");
        return _qread(p0, p1, p2);
    }

    public static void* @qrealloc(void* p0, nuint p1)
    {
        if ((nint)_qrealloc == 0) throw new System.EntryPointNotFoundException("'qrealloc' is not available for the loaded IDA SDK version.");
        return _qrealloc(p0, p1);
    }

    public static void* @qrealloc_or_throw(void* p0, nuint p1)
    {
        if ((nint)_qrealloc_or_throw == 0) throw new System.EntryPointNotFoundException("'qrealloc_or_throw' is not available for the loaded IDA SDK version.");
        return _qrealloc_or_throw(p0, p1);
    }

    public static int @qregcomp(void* p0, byte* p1, int p2)
    {
        if ((nint)_qregcomp == 0) throw new System.EntryPointNotFoundException("'qregcomp' is not available for the loaded IDA SDK version.");
        return _qregcomp(p0, p1, p2);
    }

    public static nuint @qregerror(int p0, void* p1, byte* p2, nuint p3)
    {
        if ((nint)_qregerror == 0) throw new System.EntryPointNotFoundException("'qregerror' is not available for the loaded IDA SDK version.");
        return _qregerror(p0, p1, p2, p3);
    }

    public static void @qregfree(void* p0)
    {
        if ((nint)_qregfree == 0) throw new System.EntryPointNotFoundException("'qregfree' is not available for the loaded IDA SDK version.");
        _qregfree(p0);
    }

    public static int @qrename(byte* p0, byte* p1)
    {
        if ((nint)_qrename == 0) throw new System.EntryPointNotFoundException("'qrename' is not available for the loaded IDA SDK version.");
        return _qrename(p0, p1);
    }

    public static int @qrmdir(byte* p0)
    {
        if ((nint)_qrmdir == 0) throw new System.EntryPointNotFoundException("'qrmdir' is not available for the loaded IDA SDK version.");
        return _qrmdir(p0);
    }

    public static byte @qsetenv(byte* p0, byte* p1)
    {
        if ((nint)_qsetenv == 0) throw new System.EntryPointNotFoundException("'qsetenv' is not available for the loaded IDA SDK version.");
        return _qsetenv(p0, p1);
    }

    public static void @qsleep(int p0)
    {
        if ((nint)_qsleep == 0) throw new System.EntryPointNotFoundException("'qsleep' is not available for the loaded IDA SDK version.");
        _qsleep(p0);
    }

    public static byte* @qsplitfile(byte* p0, byte** p1, byte** p2)
    {
        if ((nint)_qsplitfile == 0) throw new System.EntryPointNotFoundException("'qsplitfile' is not available for the loaded IDA SDK version.");
        return _qsplitfile(p0, p1, p2);
    }

    public static int @qstat(byte* p0, void* p1)
    {
        if ((nint)_qstat == 0) throw new System.EntryPointNotFoundException("'qstat' is not available for the loaded IDA SDK version.");
        return _qstat(p0, p1);
    }

    public static byte* @qstpncpy(byte* p0, byte* p1, nuint p2)
    {
        if ((nint)_qstpncpy == 0) throw new System.EntryPointNotFoundException("'qstpncpy' is not available for the loaded IDA SDK version.");
        return _qstpncpy(p0, p1, p2);
    }

    public static void @qstr2user(QString* p0, byte* p1, int p2)
    {
        if ((nint)_qstr2user == 0) throw new System.EntryPointNotFoundException("'qstr2user' is not available for the loaded IDA SDK version.");
        _qstr2user(p0, p1, p2);
    }

    public static int @qstrcmp(void* p0, void* p1)
    {
        if ((nint)_qstrcmp == 0) throw new System.EntryPointNotFoundException("'qstrcmp' is not available for the loaded IDA SDK version.");
        return _qstrcmp(p0, p1);
    }

    public static byte* @qstrdup(byte* p0)
    {
        if ((nint)_qstrdup == 0) throw new System.EntryPointNotFoundException("'qstrdup' is not available for the loaded IDA SDK version.");
        return _qstrdup(p0);
    }

    public static byte* @qstrerror(int p0)
    {
        if ((nint)_qstrerror == 0) throw new System.EntryPointNotFoundException("'qstrerror' is not available for the loaded IDA SDK version.");
        return _qstrerror(p0);
    }

    public static nuint @qstrlen(void* p0)
    {
        if ((nint)_qstrlen == 0) throw new System.EntryPointNotFoundException("'qstrlen' is not available for the loaded IDA SDK version.");
        return _qstrlen(p0);
    }

    public static byte* @qstrlwr(byte* p0)
    {
        if ((nint)_qstrlwr == 0) throw new System.EntryPointNotFoundException("'qstrlwr' is not available for the loaded IDA SDK version.");
        return _qstrlwr(p0);
    }

    public static byte* @qstrncat(byte* p0, byte* p1, nuint p2)
    {
        if ((nint)_qstrncat == 0) throw new System.EntryPointNotFoundException("'qstrncat' is not available for the loaded IDA SDK version.");
        return _qstrncat(p0, p1, p2);
    }

    public static int @qstrncmp(void* p0, void* p1, nuint p2)
    {
        if ((nint)_qstrncmp == 0) throw new System.EntryPointNotFoundException("'qstrncmp' is not available for the loaded IDA SDK version.");
        return _qstrncmp(p0, p1, p2);
    }

    public static byte* @qstrncpy(byte* p0, byte* p1, nuint p2)
    {
        if ((nint)_qstrncpy == 0) throw new System.EntryPointNotFoundException("'qstrncpy' is not available for the loaded IDA SDK version.");
        return _qstrncpy(p0, p1, p2);
    }

    public static byte* @qstrtok(byte* p0, byte* p1, byte** p2)
    {
        if ((nint)_qstrtok == 0) throw new System.EntryPointNotFoundException("'qstrtok' is not available for the loaded IDA SDK version.");
        return _qstrtok(p0, p1, p2);
    }

    public static byte* @qstrupr(byte* p0)
    {
        if ((nint)_qstrupr == 0) throw new System.EntryPointNotFoundException("'qstrupr' is not available for the loaded IDA SDK version.");
        return _qstrupr(p0);
    }

    public static ulong @qtime64()
    {
        if ((nint)_qtime64 == 0) throw new System.EntryPointNotFoundException("'qtime64' is not available for the loaded IDA SDK version.");
        return _qtime64();
    }

    public static byte* @qtmpdir(byte* p0, nuint p1)
    {
        if ((nint)_qtmpdir == 0) throw new System.EntryPointNotFoundException("'qtmpdir' is not available for the loaded IDA SDK version.");
        return _qtmpdir(p0, p1);
    }

    public static void* @qtmpfile()
    {
        if ((nint)_qtmpfile == 0) throw new System.EntryPointNotFoundException("'qtmpfile' is not available for the loaded IDA SDK version.");
        return _qtmpfile();
    }

    public static byte* @qtmpnam(byte* p0, nuint p1)
    {
        if ((nint)_qtmpnam == 0) throw new System.EntryPointNotFoundException("'qtmpnam' is not available for the loaded IDA SDK version.");
        return _qtmpnam(p0, p1);
    }

    public static int @qtouchfile(byte* p0)
    {
        if ((nint)_qtouchfile == 0) throw new System.EntryPointNotFoundException("'qtouchfile' is not available for the loaded IDA SDK version.");
        return _qtouchfile(p0);
    }

    public static int @qunlink(byte* p0)
    {
        if ((nint)_qunlink == 0) throw new System.EntryPointNotFoundException("'qunlink' is not available for the loaded IDA SDK version.");
        return _qunlink(p0);
    }

    public static byte @quote_cmdline_arg(QString* p0)
    {
        if ((nint)_quote_cmdline_arg == 0) throw new System.EntryPointNotFoundException("'quote_cmdline_arg' is not available for the loaded IDA SDK version.");
        return _quote_cmdline_arg(p0);
    }

    public static nuint @qustrlen(byte* p0)
    {
        if ((nint)_qustrlen == 0) throw new System.EntryPointNotFoundException("'qustrlen' is not available for the loaded IDA SDK version.");
        return _qustrlen(p0);
    }

    public static byte @qustrncpy(byte* p0, byte* p1, nuint p2)
    {
        if ((nint)_qustrncpy == 0) throw new System.EntryPointNotFoundException("'qustrncpy' is not available for the loaded IDA SDK version.");
        return _qustrncpy(p0, p1, p2);
    }

    public static void* @qvector_reserve(void* p0, void* p1, nuint p2, nuint p3)
    {
        if ((nint)_qvector_reserve == 0) throw new System.EntryPointNotFoundException("'qvector_reserve' is not available for the loaded IDA SDK version.");
        return _qvector_reserve(p0, p1, p2, p3);
    }

    public static int @qwait_for_handles(int* p0, int* p1, int p2, uint p3, int p4)
    {
        if ((nint)_qwait_for_handles == 0) throw new System.EntryPointNotFoundException("'qwait_for_handles' is not available for the loaded IDA SDK version.");
        return _qwait_for_handles(p0, p1, p2, p3, p4);
    }

    public static int @qwait_timed(int* p0, int p1, int p2, int p3)
    {
        if ((nint)_qwait_timed == 0) throw new System.EntryPointNotFoundException("'qwait_timed' is not available for the loaded IDA SDK version.");
        return _qwait_timed(p0, p1, p2, p3);
    }

    public static int @qwrite(int p0, void* p1, nuint p2)
    {
        if ((nint)_qwrite == 0) throw new System.EntryPointNotFoundException("'qwrite' is not available for the loaded IDA SDK version.");
        return _qwrite(p0, p1, p2);
    }

    public static int @r50_to_asc(byte* p0, ushort* p1, int p2)
    {
        if ((nint)_r50_to_asc == 0) throw new System.EntryPointNotFoundException("'r50_to_asc' is not available for the loaded IDA SDK version.");
        return _r50_to_asc(p0, p1, p2);
    }

    public static nuint @range_t_print(void* p0, byte* p1, nuint p2)
    {
        if ((nint)_range_t_print == 0) throw new System.EntryPointNotFoundException("'range_t_print' is not available for the loaded IDA SDK version.");
        return _range_t_print(p0, p1, p2);
    }

    public static byte @rangeset_t_add(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_add == 0) throw new System.EntryPointNotFoundException("'rangeset_t_add' is not available for the loaded IDA SDK version.");
        return _rangeset_t_add(p0, p1);
    }

    public static byte @rangeset_t_add2(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_add2 == 0) throw new System.EntryPointNotFoundException("'rangeset_t_add2' is not available for the loaded IDA SDK version.");
        return _rangeset_t_add2(p0, p1);
    }

    public static byte @rangeset_t_contains(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_contains == 0) throw new System.EntryPointNotFoundException("'rangeset_t_contains' is not available for the loaded IDA SDK version.");
        return _rangeset_t_contains(p0, p1);
    }

    public static void* @rangeset_t_find_range(void* p0, ulong p1)
    {
        if ((nint)_rangeset_t_find_range == 0) throw new System.EntryPointNotFoundException("'rangeset_t_find_range' is not available for the loaded IDA SDK version.");
        return _rangeset_t_find_range(p0, p1);
    }

    public static byte @rangeset_t_has_common(void* p0, void* p1, byte p2)
    {
        if ((nint)_rangeset_t_has_common == 0) throw new System.EntryPointNotFoundException("'rangeset_t_has_common' is not available for the loaded IDA SDK version.");
        return _rangeset_t_has_common(p0, p1, p2);
    }

    public static byte @rangeset_t_has_common2(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_has_common2 == 0) throw new System.EntryPointNotFoundException("'rangeset_t_has_common2' is not available for the loaded IDA SDK version.");
        return _rangeset_t_has_common2(p0, p1);
    }

    public static byte @rangeset_t_intersect(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_intersect == 0) throw new System.EntryPointNotFoundException("'rangeset_t_intersect' is not available for the loaded IDA SDK version.");
        return _rangeset_t_intersect(p0, p1);
    }

    public static ulong @rangeset_t_next_addr(void* p0, ulong p1)
    {
        if ((nint)_rangeset_t_next_addr == 0) throw new System.EntryPointNotFoundException("'rangeset_t_next_addr' is not available for the loaded IDA SDK version.");
        return _rangeset_t_next_addr(p0, p1);
    }

    public static ulong @rangeset_t_next_range(void* p0, ulong p1)
    {
        if ((nint)_rangeset_t_next_range == 0) throw new System.EntryPointNotFoundException("'rangeset_t_next_range' is not available for the loaded IDA SDK version.");
        return _rangeset_t_next_range(p0, p1);
    }

    public static ulong @rangeset_t_prev_addr(void* p0, ulong p1)
    {
        if ((nint)_rangeset_t_prev_addr == 0) throw new System.EntryPointNotFoundException("'rangeset_t_prev_addr' is not available for the loaded IDA SDK version.");
        return _rangeset_t_prev_addr(p0, p1);
    }

    public static ulong @rangeset_t_prev_range(void* p0, ulong p1)
    {
        if ((nint)_rangeset_t_prev_range == 0) throw new System.EntryPointNotFoundException("'rangeset_t_prev_range' is not available for the loaded IDA SDK version.");
        return _rangeset_t_prev_range(p0, p1);
    }

    public static nuint @rangeset_t_print(void* p0, byte* p1, nuint p2)
    {
        if ((nint)_rangeset_t_print == 0) throw new System.EntryPointNotFoundException("'rangeset_t_print' is not available for the loaded IDA SDK version.");
        return _rangeset_t_print(p0, p1, p2);
    }

    public static byte @rangeset_t_sub(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_sub == 0) throw new System.EntryPointNotFoundException("'rangeset_t_sub' is not available for the loaded IDA SDK version.");
        return _rangeset_t_sub(p0, p1);
    }

    public static byte @rangeset_t_sub2(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_sub2 == 0) throw new System.EntryPointNotFoundException("'rangeset_t_sub2' is not available for the loaded IDA SDK version.");
        return _rangeset_t_sub2(p0, p1);
    }

    public static void @rangeset_t_swap(void* p0, void* p1)
    {
        if ((nint)_rangeset_t_swap == 0) throw new System.EntryPointNotFoundException("'rangeset_t_swap' is not available for the loaded IDA SDK version.");
        _rangeset_t_swap(p0, p1);
    }

    public static int @read2bytes(int p0, ushort* p1, byte p2)
    {
        if ((nint)_read2bytes == 0) throw new System.EntryPointNotFoundException("'read2bytes' is not available for the loaded IDA SDK version.");
        return _read2bytes(p0, p1, p2);
    }

    public static nint @read_ioports(void* p0, QString* p1, byte* p2, void* p3)
    {
        if ((nint)_read_ioports == 0) throw new System.EntryPointNotFoundException("'read_ioports' is not available for the loaded IDA SDK version.");
        return _read_ioports(p0, p1, p2, p3);
    }

    public static void @read_regargs(void* p0)
    {
        if ((nint)_read_regargs == 0) throw new System.EntryPointNotFoundException("'read_regargs' is not available for the loaded IDA SDK version.");
        _read_regargs(p0);
    }

    public static int @read_struc_path(ulong* p0, long* p1, ulong p2, int p3)
    {
        if ((nint)_read_struc_path == 0) throw new System.EntryPointNotFoundException("'read_struc_path' is not available for the loaded IDA SDK version.");
        return _read_struc_path(p0, p1, p2, p3);
    }

    public static ulong @read_tinfo_bitfield_value(ulong p0, ulong p1, int p2)
    {
        if ((nint)_read_tinfo_bitfield_value == 0) throw new System.EntryPointNotFoundException("'read_tinfo_bitfield_value' is not available for the loaded IDA SDK version.");
        return _read_tinfo_bitfield_value(p0, p1, p2);
    }

    public static int @readbytes(int p0, uint* p1, int p2, byte p3)
    {
        if ((nint)_readbytes == 0) throw new System.EntryPointNotFoundException("'readbytes' is not available for the loaded IDA SDK version.");
        return _readbytes(p0, p1, p2, p3);
    }

    public static void @realtoasc(byte* p0, nuint p1, void* p2, uint p3)
    {
        if ((nint)_realtoasc == 0) throw new System.EntryPointNotFoundException("'realtoasc' is not available for the loaded IDA SDK version.");
        _realtoasc(p0, p1, p2, p3);
    }

    public static void @reanalyze_callers(ulong p0, byte p1)
    {
        if ((nint)_reanalyze_callers == 0) throw new System.EntryPointNotFoundException("'reanalyze_callers' is not available for the loaded IDA SDK version.");
        _reanalyze_callers(p0, p1);
    }

    public static void @reanalyze_function(void* p0, ulong p1, ulong p2, byte p3)
    {
        if ((nint)_reanalyze_function == 0) throw new System.EntryPointNotFoundException("'reanalyze_function' is not available for the loaded IDA SDK version.");
        _reanalyze_function(p0, p1, p2, p3);
    }

    public static void @reanalyze_function_ea(ulong p0, ulong p1, ulong p2, byte p3)
    {
        if ((nint)_reanalyze_function_ea == 0) throw new System.EntryPointNotFoundException("'reanalyze_function_ea' is not available for the loaded IDA SDK version.");
        _reanalyze_function_ea(p0, p1, p2, p3);
    }

    public static byte @reanalyze_noret_flag(ulong p0)
    {
        if ((nint)_reanalyze_noret_flag == 0) throw new System.EntryPointNotFoundException("'reanalyze_noret_flag' is not available for the loaded IDA SDK version.");
        return _reanalyze_noret_flag(p0);
    }

    public static int @rebase_program(long p0, int p1)
    {
        if ((nint)_rebase_program == 0) throw new System.EntryPointNotFoundException("'rebase_program' is not available for the loaded IDA SDK version.");
        return _rebase_program(p0, p1);
    }

    public static void @rebuild_nlist()
    {
        if ((nint)_rebuild_nlist == 0) throw new System.EntryPointNotFoundException("'rebuild_nlist' is not available for the loaded IDA SDK version.");
        _rebuild_nlist();
    }

    public static byte @recalc_func_spd_for_basic_block(ulong p0, ulong p1)
    {
        if ((nint)_recalc_func_spd_for_basic_block == 0) throw new System.EntryPointNotFoundException("'recalc_func_spd_for_basic_block' is not available for the loaded IDA SDK version.");
        return _recalc_func_spd_for_basic_block(p0, p1);
    }

    public static byte @recalc_spd(ulong p0)
    {
        if ((nint)_recalc_spd == 0) throw new System.EntryPointNotFoundException("'recalc_spd' is not available for the loaded IDA SDK version.");
        return _recalc_spd(p0);
    }

    public static byte @recalc_spd_for_basic_block(void* p0, ulong p1)
    {
        if ((nint)_recalc_spd_for_basic_block == 0) throw new System.EntryPointNotFoundException("'recalc_spd_for_basic_block' is not available for the loaded IDA SDK version.");
        return _recalc_spd_for_basic_block(p0, p1);
    }

    public static byte @reg_bin_op(byte* p0, byte p1, void* p2, nuint p3, byte* p4, int p5)
    {
        if ((nint)_reg_bin_op == 0) throw new System.EntryPointNotFoundException("'reg_bin_op' is not available for the loaded IDA SDK version.");
        return _reg_bin_op(p0, p1, p2, p3, p4, p5);
    }

    public static byte @reg_data_type(int* p0, byte* p1, byte* p2)
    {
        if ((nint)_reg_data_type == 0) throw new System.EntryPointNotFoundException("'reg_data_type' is not available for the loaded IDA SDK version.");
        return _reg_data_type(p0, p1, p2);
    }

    public static byte @reg_delete(byte* p0, byte* p1)
    {
        if ((nint)_reg_delete == 0) throw new System.EntryPointNotFoundException("'reg_delete' is not available for the loaded IDA SDK version.");
        return _reg_delete(p0, p1);
    }

    public static byte @reg_delete_subkey(byte* p0)
    {
        if ((nint)_reg_delete_subkey == 0) throw new System.EntryPointNotFoundException("'reg_delete_subkey' is not available for the loaded IDA SDK version.");
        return _reg_delete_subkey(p0);
    }

    public static byte @reg_delete_tree(byte* p0)
    {
        if ((nint)_reg_delete_tree == 0) throw new System.EntryPointNotFoundException("'reg_delete_tree' is not available for the loaded IDA SDK version.");
        return _reg_delete_tree(p0);
    }

    public static byte @reg_exists(byte* p0, byte* p1)
    {
        if ((nint)_reg_exists == 0) throw new System.EntryPointNotFoundException("'reg_exists' is not available for the loaded IDA SDK version.");
        return _reg_exists(p0, p1);
    }

    public static byte @reg_finder94_find_reg_value_info(void* p0, ulong p1, int p2, int p3)
    {
        if ((nint)_reg_finder94_find_reg_value_info == 0) throw new System.EntryPointNotFoundException("'reg_finder94_find_reg_value_info' is not available for the loaded IDA SDK version.");
        return _reg_finder94_find_reg_value_info(p0, p1, p2, p3);
    }

    public static void @reg_finder94_make_rfop(void* p0, void* p1, void* p2, void* p3, ulong p4)
    {
        if ((nint)_reg_finder94_make_rfop == 0) throw new System.EntryPointNotFoundException("'reg_finder94_make_rfop' is not available for the loaded IDA SDK version.");
        _reg_finder94_make_rfop(p0, p1, p2, p3, p4);
    }

    public static byte @reg_finder_calc_op_addr(void* p0, void* p1, void* p2, void* p3, ulong p4, ulong p5, int p6)
    {
        if ((nint)_reg_finder_calc_op_addr == 0) throw new System.EntryPointNotFoundException("'reg_finder_calc_op_addr' is not available for the loaded IDA SDK version.");
        return _reg_finder_calc_op_addr(p0, p1, p2, p3, p4, p5, p6);
    }

    public static byte @reg_finder_can_resolve_mem(void* p0, ulong p1)
    {
        if ((nint)_reg_finder_can_resolve_mem == 0) throw new System.EntryPointNotFoundException("'reg_finder_can_resolve_mem' is not available for the loaded IDA SDK version.");
        return _reg_finder_can_resolve_mem(p0, p1);
    }

    public static void @reg_finder_ctr(void* p0)
    {
        if ((nint)_reg_finder_ctr == 0) throw new System.EntryPointNotFoundException("'reg_finder_ctr' is not available for the loaded IDA SDK version.");
        _reg_finder_ctr(p0);
    }

    public static void @reg_finder_dtr(void* p0)
    {
        if ((nint)_reg_finder_dtr == 0) throw new System.EntryPointNotFoundException("'reg_finder_dtr' is not available for the loaded IDA SDK version.");
        _reg_finder_dtr(p0);
    }

    public static void @reg_finder_emulate_binary_op_shifted(void* p0, void* p1, int p2, void* p3, void* p4, int p5, byte p6, int p7, byte p8, void* p9, ulong p10, ulong p11)
    {
        if ((nint)_reg_finder_emulate_binary_op_shifted == 0) throw new System.EntryPointNotFoundException("'reg_finder_emulate_binary_op_shifted' is not available for the loaded IDA SDK version.");
        _reg_finder_emulate_binary_op_shifted(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static byte @reg_finder_emulate_mem_read(void* p0, void* p1, void* p2, int p3, byte p4, void* p5)
    {
        if ((nint)_reg_finder_emulate_mem_read == 0) throw new System.EntryPointNotFoundException("'reg_finder_emulate_mem_read' is not available for the loaded IDA SDK version.");
        return _reg_finder_emulate_mem_read(p0, p1, p2, p3, p4, p5);
    }

    public static void @reg_finder_emulate_unary_op(void* p0, void* p1, int p2, int p3, void* p4, ulong p5, ulong p6)
    {
        if ((nint)_reg_finder_emulate_unary_op == 0) throw new System.EntryPointNotFoundException("'reg_finder_emulate_unary_op' is not available for the loaded IDA SDK version.");
        _reg_finder_emulate_unary_op(p0, p1, p2, p3, p4, p5, p6);
    }

    public static void @reg_finder_invalidate_cache(void* p0, ulong p1, ulong p2, int p3)
    {
        if ((nint)_reg_finder_invalidate_cache == 0) throw new System.EntryPointNotFoundException("'reg_finder_invalidate_cache' is not available for the loaded IDA SDK version.");
        _reg_finder_invalidate_cache(p0, p1, p2, p3);
    }

    public static void @reg_finder_invalidate_xrefs_cache(void* p0, ulong p1, int p2)
    {
        if ((nint)_reg_finder_invalidate_xrefs_cache == 0) throw new System.EntryPointNotFoundException("'reg_finder_invalidate_xrefs_cache' is not available for the loaded IDA SDK version.");
        _reg_finder_invalidate_xrefs_cache(p0, p1, p2);
    }

    public static void @reg_finder_make_rfop(void* p0, void* p1, void* p2, void* p3, void* p4)
    {
        if ((nint)_reg_finder_make_rfop == 0) throw new System.EntryPointNotFoundException("'reg_finder_make_rfop' is not available for the loaded IDA SDK version.");
        _reg_finder_make_rfop(p0, p1, p2, p3, p4);
    }

    public static int @reg_int_op(byte* p0, byte p1, int p2, byte* p3)
    {
        if ((nint)_reg_int_op == 0) throw new System.EntryPointNotFoundException("'reg_int_op' is not available for the loaded IDA SDK version.");
        return _reg_int_op(p0, p1, p2, p3);
    }

    public static void @reg_read_strlist(void* p0, byte* p1)
    {
        if ((nint)_reg_read_strlist == 0) throw new System.EntryPointNotFoundException("'reg_read_strlist' is not available for the loaded IDA SDK version.");
        _reg_read_strlist(p0, p1);
    }

    public static byte @reg_str_get(QString* p0, byte* p1, byte* p2)
    {
        if ((nint)_reg_str_get == 0) throw new System.EntryPointNotFoundException("'reg_str_get' is not available for the loaded IDA SDK version.");
        return _reg_str_get(p0, p1, p2);
    }

    public static void @reg_str_set(byte* p0, byte* p1, byte* p2)
    {
        if ((nint)_reg_str_set == 0) throw new System.EntryPointNotFoundException("'reg_str_set' is not available for the loaded IDA SDK version.");
        _reg_str_set(p0, p1, p2);
    }

    public static byte @reg_subkey_children(void* p0, byte* p1, byte p2)
    {
        if ((nint)_reg_subkey_children == 0) throw new System.EntryPointNotFoundException("'reg_subkey_children' is not available for the loaded IDA SDK version.");
        return _reg_subkey_children(p0, p1, p2);
    }

    public static byte @reg_subkey_exists(byte* p0)
    {
        if ((nint)_reg_subkey_exists == 0) throw new System.EntryPointNotFoundException("'reg_subkey_exists' is not available for the loaded IDA SDK version.");
        return _reg_subkey_exists(p0);
    }

    public static void @reg_update_strlist(byte* p0, byte* p1, nuint p2, byte* p3, byte p4)
    {
        if ((nint)_reg_update_strlist == 0) throw new System.EntryPointNotFoundException("'reg_update_strlist' is not available for the loaded IDA SDK version.");
        _reg_update_strlist(p0, p1, p2, p3, p4);
    }

    public static void @reg_value_base_dstr(void* p0, QString* p1, void* p2)
    {
        if ((nint)_reg_value_base_dstr == 0) throw new System.EntryPointNotFoundException("'reg_value_base_dstr' is not available for the loaded IDA SDK version.");
        _reg_value_base_dstr(p0, p1, p2);
    }

    public static int @reg_value_base_vals_union(void* p0, void* p1)
    {
        if ((nint)_reg_value_base_vals_union == 0) throw new System.EntryPointNotFoundException("'reg_value_base_vals_union' is not available for the loaded IDA SDK version.");
        return _reg_value_base_vals_union(p0, p1);
    }

    public static void @reg_value_def_dstr(void* p0, QString* p1, int p2, void* p3)
    {
        if ((nint)_reg_value_def_dstr == 0) throw new System.EntryPointNotFoundException("'reg_value_def_dstr' is not available for the loaded IDA SDK version.");
        _reg_value_def_dstr(p0, p1, p2, p3);
    }

    public static void @reg_value_info_dstr(void* p0, QString* p1, void* p2)
    {
        if ((nint)_reg_value_info_dstr == 0) throw new System.EntryPointNotFoundException("'reg_value_info_dstr' is not available for the loaded IDA SDK version.");
        _reg_value_info_dstr(p0, p1, p2);
    }

    public static int @reg_value_info_vals_union(void* p0, void* p1)
    {
        if ((nint)_reg_value_info_vals_union == 0) throw new System.EntryPointNotFoundException("'reg_value_info_vals_union' is not available for the loaded IDA SDK version.");
        return _reg_value_info_vals_union(p0, p1);
    }

    public static void @reg_write_strlist(void* p0, byte* p1)
    {
        if ((nint)_reg_write_strlist == 0) throw new System.EntryPointNotFoundException("'reg_write_strlist' is not available for the loaded IDA SDK version.");
        _reg_write_strlist(p0, p1);
    }

    public static int @regarg_t__compare(void* p0, void* p1)
    {
        if ((nint)_regarg_t__compare == 0) throw new System.EntryPointNotFoundException("'regarg_t__compare' is not available for the loaded IDA SDK version.");
        return _regarg_t__compare(p0, p1);
    }

    public static int @regex_match(byte* p0, byte* p1, byte p2)
    {
        if ((nint)_regex_match == 0) throw new System.EntryPointNotFoundException("'regex_match' is not available for the loaded IDA SDK version.");
        return _regex_match(p0, p1, p2);
    }

    public static uint @register_custom_callcnv(void* p0)
    {
        if ((nint)_register_custom_callcnv == 0) throw new System.EntryPointNotFoundException("'register_custom_callcnv' is not available for the loaded IDA SDK version.");
        return _register_custom_callcnv(p0);
    }

    public static int @register_custom_data_format(void* p0)
    {
        if ((nint)_register_custom_data_format == 0) throw new System.EntryPointNotFoundException("'register_custom_data_format' is not available for the loaded IDA SDK version.");
        return _register_custom_data_format(p0);
    }

    public static int @register_custom_data_type(void* p0)
    {
        if ((nint)_register_custom_data_type == 0) throw new System.EntryPointNotFoundException("'register_custom_data_type' is not available for the loaded IDA SDK version.");
        return _register_custom_data_type(p0);
    }

    public static ushort @register_custom_fixup(void* p0)
    {
        if ((nint)_register_custom_fixup == 0) throw new System.EntryPointNotFoundException("'register_custom_fixup' is not available for the loaded IDA SDK version.");
        return _register_custom_fixup(p0);
    }

    public static int @register_custom_refinfo(void* p0)
    {
        if ((nint)_register_custom_refinfo == 0) throw new System.EntryPointNotFoundException("'register_custom_refinfo' is not available for the loaded IDA SDK version.");
        return _register_custom_refinfo(p0);
    }

    public static void @register_loc_converter2(byte* p0, byte* p1, void* p2)
    {
        if ((nint)_register_loc_converter2 == 0) throw new System.EntryPointNotFoundException("'register_loc_converter2' is not available for the loaded IDA SDK version.");
        _register_loc_converter2(p0, p1, p2);
    }

    public static byte @register_post_event_visitor(int p0, void* p1, void* p2)
    {
        if ((nint)_register_post_event_visitor == 0) throw new System.EntryPointNotFoundException("'register_post_event_visitor' is not available for the loaded IDA SDK version.");
        return _register_post_event_visitor(p0, p1, p2);
    }

    public static int @regvar_t__compare(void* p0, void* p1)
    {
        if ((nint)_regvar_t__compare == 0) throw new System.EntryPointNotFoundException("'regvar_t__compare' is not available for the loaded IDA SDK version.");
        return _regvar_t__compare(p0, p1);
    }

    public static byte @reload_file(byte* p0, byte p1)
    {
        if ((nint)_reload_file == 0) throw new System.EntryPointNotFoundException("'reload_file' is not available for the loaded IDA SDK version.");
        return _reload_file(p0, p1);
    }

    public static void @reloc_value(void* p0, int p1, long p2, byte p3)
    {
        if ((nint)_reloc_value == 0) throw new System.EntryPointNotFoundException("'reloc_value' is not available for the loaded IDA SDK version.");
        _reloc_value(p0, p1, p2, p3);
    }

    public static byte @relocate_relobj(void* p0, ulong p1, byte p2)
    {
        if ((nint)_relocate_relobj == 0) throw new System.EntryPointNotFoundException("'relocate_relobj' is not available for the loaded IDA SDK version.");
        return _relocate_relobj(p0, p1, p2);
    }

    public static void @remember_problem(byte p0, ulong p1, byte* p2)
    {
        if ((nint)_remember_problem == 0) throw new System.EntryPointNotFoundException("'remember_problem' is not available for the loaded IDA SDK version.");
        _remember_problem(p0, p1, p2);
    }

    public static byte @remove_abi_opts(byte* p0, byte p1)
    {
        if ((nint)_remove_abi_opts == 0) throw new System.EntryPointNotFoundException("'remove_abi_opts' is not available for the loaded IDA SDK version.");
        return _remove_abi_opts(p0, p1);
    }

    public static byte @remove_custom_argloc(int p0)
    {
        if ((nint)_remove_custom_argloc == 0) throw new System.EntryPointNotFoundException("'remove_custom_argloc' is not available for the loaded IDA SDK version.");
        return _remove_custom_argloc(p0);
    }

    public static void @remove_event_listener(void* p0)
    {
        if ((nint)_remove_event_listener == 0) throw new System.EntryPointNotFoundException("'remove_event_listener' is not available for the loaded IDA SDK version.");
        _remove_event_listener(p0);
    }

    public static byte @remove_extlang(void* p0)
    {
        if ((nint)_remove_extlang == 0) throw new System.EntryPointNotFoundException("'remove_extlang' is not available for the loaded IDA SDK version.");
        return _remove_extlang(p0);
    }

    public static byte @remove_func_tail(void* p0, ulong p1)
    {
        if ((nint)_remove_func_tail == 0) throw new System.EntryPointNotFoundException("'remove_func_tail' is not available for the loaded IDA SDK version.");
        return _remove_func_tail(p0, p1);
    }

    public static byte @remove_func_tail_ea(ulong p0, ulong p1)
    {
        if ((nint)_remove_func_tail_ea == 0) throw new System.EntryPointNotFoundException("'remove_func_tail_ea' is not available for the loaded IDA SDK version.");
        return _remove_func_tail_ea(p0, p1);
    }

    public static byte @remove_tinfo_pointer(TypeInfo* p0, byte** p1, void* p2)
    {
        if ((nint)_remove_tinfo_pointer == 0) throw new System.EntryPointNotFoundException("'remove_tinfo_pointer' is not available for the loaded IDA SDK version.");
        return _remove_tinfo_pointer(p0, p1, p2);
    }

    public static byte @rename_encoding(int p0, byte* p1)
    {
        if ((nint)_rename_encoding == 0) throw new System.EntryPointNotFoundException("'rename_encoding' is not available for the loaded IDA SDK version.");
        return _rename_encoding(p0, p1);
    }

    public static byte @rename_entry(ulong p0, byte* p1, int p2)
    {
        if ((nint)_rename_entry == 0) throw new System.EntryPointNotFoundException("'rename_entry' is not available for the loaded IDA SDK version.");
        return _rename_entry(p0, p1, p2);
    }

    public static int @rename_func_regvar(ulong p0, nint p1, byte* p2)
    {
        if ((nint)_rename_func_regvar == 0) throw new System.EntryPointNotFoundException("'rename_func_regvar' is not available for the loaded IDA SDK version.");
        return _rename_func_regvar(p0, p1, p2);
    }

    public static int @rename_regvar(void* p0, void* p1, byte* p2)
    {
        if ((nint)_rename_regvar == 0) throw new System.EntryPointNotFoundException("'rename_regvar' is not available for the loaded IDA SDK version.");
        return _rename_regvar(p0, p1, p2);
    }

    public static void @reorder_dummy_names()
    {
        if ((nint)_reorder_dummy_names == 0) throw new System.EntryPointNotFoundException("'reorder_dummy_names' is not available for the loaded IDA SDK version.");
        _reorder_dummy_names();
    }

    public static int @replace_ordinal_typerefs(void* p0, TypeInfo* p1)
    {
        if ((nint)_replace_ordinal_typerefs == 0) throw new System.EntryPointNotFoundException("'replace_ordinal_typerefs' is not available for the loaded IDA SDK version.");
        return _replace_ordinal_typerefs(p0, p1);
    }

    public static byte @replace_tabs(QString* p0, byte* p1, int p2)
    {
        if ((nint)_replace_tabs == 0) throw new System.EntryPointNotFoundException("'replace_tabs' is not available for the loaded IDA SDK version.");
        return _replace_tabs(p0, p1, p2);
    }

    public static void @request_refresh(ulong p0, byte p1)
    {
        if ((nint)_request_refresh == 0) throw new System.EntryPointNotFoundException("'request_refresh' is not available for the loaded IDA SDK version.");
        _request_refresh(p0, p1);
    }

    public static void @reset_dirtree(void* p0)
    {
        if ((nint)_reset_dirtree == 0) throw new System.EntryPointNotFoundException("'reset_dirtree' is not available for the loaded IDA SDK version.");
        _reset_dirtree(p0);
    }

    public static byte @resolve_field_path(void* p0, void* p1, byte* p2)
    {
        if ((nint)_resolve_field_path == 0) throw new System.EntryPointNotFoundException("'resolve_field_path' is not available for the loaded IDA SDK version.");
        return _resolve_field_path(p0, p1, p2);
    }

    public static byte* @resolve_typedef(void* p0, byte* p1)
    {
        if ((nint)_resolve_typedef == 0) throw new System.EntryPointNotFoundException("'resolve_typedef' is not available for the loaded IDA SDK version.");
        return _resolve_typedef(p0, p1);
    }

    public static void* @retrieve_custom_argloc(int p0)
    {
        if ((nint)_retrieve_custom_argloc == 0) throw new System.EntryPointNotFoundException("'retrieve_custom_argloc' is not available for the loaded IDA SDK version.");
        return _retrieve_custom_argloc(p0);
    }

    public static byte @revert_byte(ulong p0)
    {
        if ((nint)_revert_byte == 0) throw new System.EntryPointNotFoundException("'revert_byte' is not available for the loaded IDA SDK version.");
        return _revert_byte(p0);
    }

    public static void @revert_ida_decisions(ulong p0, ulong p1)
    {
        if ((nint)_revert_ida_decisions == 0) throw new System.EntryPointNotFoundException("'revert_ida_decisions' is not available for the loaded IDA SDK version.");
        _revert_ida_decisions(p0, p1);
    }

    public static byte @revert_metadata(ulong p0)
    {
        if ((nint)_revert_metadata == 0) throw new System.EntryPointNotFoundException("'revert_metadata' is not available for the loaded IDA SDK version.");
        return _revert_metadata(p0);
    }

    public static ulong @rotate_left(ulong p0, int p1, nuint p2, nuint p3)
    {
        if ((nint)_rotate_left == 0) throw new System.EntryPointNotFoundException("'rotate_left' is not available for the loaded IDA SDK version.");
        return _rotate_left(p0, p1, p2, p3);
    }

    public static byte @run_plugin(void* p0, nuint p1)
    {
        if ((nint)_run_plugin == 0) throw new System.EntryPointNotFoundException("'run_plugin' is not available for the loaded IDA SDK version.");
        return _run_plugin(p0, p1);
    }

    public static byte @same_value_jpt(void* p0, void* p1, int p2)
    {
        if ((nint)_same_value_jpt == 0) throw new System.EntryPointNotFoundException("'same_value_jpt' is not available for the loaded IDA SDK version.");
        return _same_value_jpt(p0, p1, p2);
    }

    public static byte @sanitize_file_name(byte* p0, nuint p1)
    {
        if ((nint)_sanitize_file_name == 0) throw new System.EntryPointNotFoundException("'sanitize_file_name' is not available for the loaded IDA SDK version.");
        return _sanitize_file_name(p0, p1);
    }

    public static byte @save_database(byte* p0, uint p1, void* p2, void* p3)
    {
        if ((nint)_save_database == 0) throw new System.EntryPointNotFoundException("'save_database' is not available for the loaded IDA SDK version.");
        return _save_database(p0, p1, p2, p3);
    }

    public static byte @save_dirtree(void* p0)
    {
        if ((nint)_save_dirtree == 0) throw new System.EntryPointNotFoundException("'save_dirtree' is not available for the loaded IDA SDK version.");
        return _save_dirtree(p0);
    }

    public static int @save_tinfo(TypeInfo* p0, void* p1, nuint p2, byte* p3, int p4)
    {
        if ((nint)_save_tinfo == 0) throw new System.EntryPointNotFoundException("'save_tinfo' is not available for the loaded IDA SDK version.");
        return _save_tinfo(p0, p1, p2, p3, p4);
    }

    public static uint @score_metadata(void* p0)
    {
        if ((nint)_score_metadata == 0) throw new System.EntryPointNotFoundException("'score_metadata' is not available for the loaded IDA SDK version.");
        return _score_metadata(p0);
    }

    public static uint @score_tinfo(TypeInfo* p0)
    {
        if ((nint)_score_tinfo == 0) throw new System.EntryPointNotFoundException("'score_tinfo' is not available for the loaded IDA SDK version.");
        return _score_tinfo(p0);
    }

    public static int @search(void* p0, void* p1, void* p2, int* p3, byte* p4, int p5)
    {
        if ((nint)_search == 0) throw new System.EntryPointNotFoundException("'search' is not available for the loaded IDA SDK version.");
        return _search(p0, p1, p2, p3, p4, p5);
    }

    public static byte @search_path(byte* p0, nuint p1, byte* p2, byte p3)
    {
        if ((nint)_search_path == 0) throw new System.EntryPointNotFoundException("'search_path' is not available for the loaded IDA SDK version.");
        return _search_path(p0, p1, p2, p3);
    }

    public static long @segm_adjust_diff(void* p0, long p1)
    {
        if ((nint)_segm_adjust_diff == 0) throw new System.EntryPointNotFoundException("'segm_adjust_diff' is not available for the loaded IDA SDK version.");
        return _segm_adjust_diff(p0, p1);
    }

    public static ulong @segm_adjust_ea(void* p0, ulong p1)
    {
        if ((nint)_segm_adjust_ea == 0) throw new System.EntryPointNotFoundException("'segm_adjust_ea' is not available for the loaded IDA SDK version.");
        return _segm_adjust_ea(p0, p1);
    }

    public static byte @segment_info_t__visible_name(void* p0, QString* p1)
    {
        if ((nint)_segment_info_t__visible_name == 0) throw new System.EntryPointNotFoundException("'segment_info_t__visible_name' is not available for the loaded IDA SDK version.");
        return _segment_info_t__visible_name(p0, p1);
    }

    public static byte @segtype(ulong p0)
    {
        if ((nint)_segtype == 0) throw new System.EntryPointNotFoundException("'segtype' is not available for the loaded IDA SDK version.");
        return _segtype(p0);
    }

    public static ulong @sel2para(ulong p0)
    {
        if ((nint)_sel2para == 0) throw new System.EntryPointNotFoundException("'sel2para' is not available for the loaded IDA SDK version.");
        return _sel2para(p0);
    }

    public static byte @select_extlang(void* p0)
    {
        if ((nint)_select_extlang == 0) throw new System.EntryPointNotFoundException("'select_extlang' is not available for the loaded IDA SDK version.");
        return _select_extlang(p0);
    }

    public static byte @select_parser_by_name(byte* p0)
    {
        if ((nint)_select_parser_by_name == 0) throw new System.EntryPointNotFoundException("'select_parser_by_name' is not available for the loaded IDA SDK version.");
        return _select_parser_by_name(p0);
    }

    public static byte @select_parser_by_srclang(int p0)
    {
        if ((nint)_select_parser_by_srclang == 0) throw new System.EntryPointNotFoundException("'select_parser_by_srclang' is not available for the loaded IDA SDK version.");
        return _select_parser_by_srclang(p0);
    }

    public static void @serialize_dynamic_register_set(void* p0, void* p1)
    {
        if ((nint)_serialize_dynamic_register_set == 0) throw new System.EntryPointNotFoundException("'serialize_dynamic_register_set' is not available for the loaded IDA SDK version.");
        _serialize_dynamic_register_set(p0, p1);
    }

    public static void @serialize_insn(void* p0, void* p1)
    {
        if ((nint)_serialize_insn == 0) throw new System.EntryPointNotFoundException("'serialize_insn' is not available for the loaded IDA SDK version.");
        _serialize_insn(p0, p1);
    }

    public static byte @serialize_json(QString* p0, void* p1, uint p2)
    {
        if ((nint)_serialize_json == 0) throw new System.EntryPointNotFoundException("'serialize_json' is not available for the loaded IDA SDK version.");
        return _serialize_json(p0, p1, p2);
    }

    public static byte @serialize_tinfo(void* p0, void* p1, void* p2, TypeInfo* p3, int p4)
    {
        if ((nint)_serialize_tinfo == 0) throw new System.EntryPointNotFoundException("'serialize_tinfo' is not available for the loaded IDA SDK version.");
        return _serialize_tinfo(p0, p1, p2, p3, p4);
    }

    public static void @set_abits(ulong p0, uint p1)
    {
        if ((nint)_set_abits == 0) throw new System.EntryPointNotFoundException("'set_abits' is not available for the loaded IDA SDK version.");
        _set_abits(p0, p1);
    }

    public static void @set_aflags(ulong p0, uint p1)
    {
        if ((nint)_set_aflags == 0) throw new System.EntryPointNotFoundException("'set_aflags' is not available for the loaded IDA SDK version.");
        _set_aflags(p0, p1);
    }

    public static void @set_array_parameters(ulong p0, void* p1)
    {
        if ((nint)_set_array_parameters == 0) throw new System.EntryPointNotFoundException("'set_array_parameters' is not available for the loaded IDA SDK version.");
        _set_array_parameters(p0, p1);
    }

    public static byte @set_auto_spd(void* p0, ulong p1, long p2)
    {
        if ((nint)_set_auto_spd == 0) throw new System.EntryPointNotFoundException("'set_auto_spd' is not available for the loaded IDA SDK version.");
        return _set_auto_spd(p0, p1, p2);
    }

    public static int @set_auto_state(int p0)
    {
        if ((nint)_set_auto_state == 0) throw new System.EntryPointNotFoundException("'set_auto_state' is not available for the loaded IDA SDK version.");
        return _set_auto_state(p0);
    }

    public static byte @set_cmt(ulong p0, byte* p1, byte p2)
    {
        if ((nint)_set_cmt == 0) throw new System.EntryPointNotFoundException("'set_cmt' is not available for the loaded IDA SDK version.");
        return _set_cmt(p0, p1, p2);
    }

    public static byte @set_compiler(void* p0, int p1, byte* p2)
    {
        if ((nint)_set_compiler == 0) throw new System.EntryPointNotFoundException("'set_compiler' is not available for the loaded IDA SDK version.");
        return _set_compiler(p0, p1, p2);
    }

    public static byte @set_compiler_string(byte* p0, byte p1)
    {
        if ((nint)_set_compiler_string == 0) throw new System.EntryPointNotFoundException("'set_compiler_string' is not available for the loaded IDA SDK version.");
        return _set_compiler_string(p0, p1);
    }

    public static void @set_cp_validity(int p0, uint p1, uint p2, byte p3)
    {
        if ((nint)_set_cp_validity == 0) throw new System.EntryPointNotFoundException("'set_cp_validity' is not available for the loaded IDA SDK version.");
        _set_cp_validity(p0, p1, p2, p3);
    }

    public static void @set_custom_data_type_ids(ulong p0, void* p1)
    {
        if ((nint)_set_custom_data_type_ids == 0) throw new System.EntryPointNotFoundException("'set_custom_data_type_ids' is not available for the loaded IDA SDK version.");
        _set_custom_data_type_ids(p0, p1);
    }

    public static void @set_database_flag(uint p0, byte p1)
    {
        if ((nint)_set_database_flag == 0) throw new System.EntryPointNotFoundException("'set_database_flag' is not available for the loaded IDA SDK version.");
        _set_database_flag(p0, p1);
    }

    public static void @set_debug_event_code(void* p0, int p1)
    {
        if ((nint)_set_debug_event_code == 0) throw new System.EntryPointNotFoundException("'set_debug_event_code' is not available for the loaded IDA SDK version.");
        _set_debug_event_code(p0, p1);
    }

    public static byte @set_debug_name(ulong p0, byte* p1)
    {
        if ((nint)_set_debug_name == 0) throw new System.EntryPointNotFoundException("'set_debug_name' is not available for the loaded IDA SDK version.");
        return _set_debug_name(p0, p1);
    }

    public static int @set_debug_names(ulong* p0, byte** p1, int p2)
    {
        if ((nint)_set_debug_names == 0) throw new System.EntryPointNotFoundException("'set_debug_names' is not available for the loaded IDA SDK version.");
        return _set_debug_names(p0, p1, p2);
    }

    public static void @set_default_dataseg(ulong p0)
    {
        if ((nint)_set_default_dataseg == 0) throw new System.EntryPointNotFoundException("'set_default_dataseg' is not available for the loaded IDA SDK version.");
        _set_default_dataseg(p0);
    }

    public static byte @set_default_encoding_idx(int p0, int p1)
    {
        if ((nint)_set_default_encoding_idx == 0) throw new System.EntryPointNotFoundException("'set_default_encoding_idx' is not available for the loaded IDA SDK version.");
        return _set_default_encoding_idx(p0, p1);
    }

    public static byte @set_default_sreg_value(void* p0, int p1, ulong p2)
    {
        if ((nint)_set_default_sreg_value == 0) throw new System.EntryPointNotFoundException("'set_default_sreg_value' is not available for the loaded IDA SDK version.");
        return _set_default_sreg_value(p0, p1, p2);
    }

    public static byte @set_default_sreg_value_ea(ulong p0, int p1, ulong p2)
    {
        if ((nint)_set_default_sreg_value_ea == 0) throw new System.EntryPointNotFoundException("'set_default_sreg_value_ea' is not available for the loaded IDA SDK version.");
        return _set_default_sreg_value_ea(p0, p1, p2);
    }

    public static byte @set_dummy_name(ulong p0, ulong p1)
    {
        if ((nint)_set_dummy_name == 0) throw new System.EntryPointNotFoundException("'set_dummy_name' is not available for the loaded IDA SDK version.");
        return _set_dummy_name(p0, p1);
    }

    public static byte @set_entry_forwarder(ulong p0, byte* p1, int p2)
    {
        if ((nint)_set_entry_forwarder == 0) throw new System.EntryPointNotFoundException("'set_entry_forwarder' is not available for the loaded IDA SDK version.");
        return _set_entry_forwarder(p0, p1, p2);
    }

    public static void @set_error_data(int p0, nuint p1)
    {
        if ((nint)_set_error_data == 0) throw new System.EntryPointNotFoundException("'set_error_data' is not available for the loaded IDA SDK version.");
        _set_error_data(p0, p1);
    }

    public static void @set_error_string(int p0, byte* p1)
    {
        if ((nint)_set_error_string == 0) throw new System.EntryPointNotFoundException("'set_error_string' is not available for the loaded IDA SDK version.");
        _set_error_string(p0, p1);
    }

    public static byte* @set_file_ext(byte* p0, nuint p1, byte* p2, byte* p3)
    {
        if ((nint)_set_file_ext == 0) throw new System.EntryPointNotFoundException("'set_file_ext' is not available for the loaded IDA SDK version.");
        return _set_file_ext(p0, p1, p2, p3);
    }

    public static void @set_fixup(ulong p0, void* p1)
    {
        if ((nint)_set_fixup == 0) throw new System.EntryPointNotFoundException("'set_fixup' is not available for the loaded IDA SDK version.");
        _set_fixup(p0, p1);
    }

    public static byte @set_forced_operand(ulong p0, int p1, byte* p2)
    {
        if ((nint)_set_forced_operand == 0) throw new System.EntryPointNotFoundException("'set_forced_operand' is not available for the loaded IDA SDK version.");
        return _set_forced_operand(p0, p1, p2);
    }

    public static byte @set_frame_member_type(void* p0, ulong p1, TypeInfo* p2, void* p3, uint p4)
    {
        if ((nint)_set_frame_member_type == 0) throw new System.EntryPointNotFoundException("'set_frame_member_type' is not available for the loaded IDA SDK version.");
        return _set_frame_member_type(p0, p1, p2, p3, p4);
    }

    public static byte @set_frame_member_type_ea(ulong p0, ulong p1, TypeInfo* p2, void* p3, uint p4)
    {
        if ((nint)_set_frame_member_type_ea == 0) throw new System.EntryPointNotFoundException("'set_frame_member_type_ea' is not available for the loaded IDA SDK version.");
        return _set_frame_member_type_ea(p0, p1, p2, p3, p4);
    }

    public static byte @set_frame_size(void* p0, ulong p1, ushort p2, ulong p3)
    {
        if ((nint)_set_frame_size == 0) throw new System.EntryPointNotFoundException("'set_frame_size' is not available for the loaded IDA SDK version.");
        return _set_frame_size(p0, p1, p2, p3);
    }

    public static byte @set_frame_size_ea(ulong p0, ulong p1, ushort p2, ulong p3)
    {
        if ((nint)_set_frame_size_ea == 0) throw new System.EntryPointNotFoundException("'set_frame_size_ea' is not available for the loaded IDA SDK version.");
        return _set_frame_size_ea(p0, p1, p2, p3);
    }

    public static byte @set_func_auto_spd(ulong p0, ulong p1, long p2)
    {
        if ((nint)_set_func_auto_spd == 0) throw new System.EntryPointNotFoundException("'set_func_auto_spd' is not available for the loaded IDA SDK version.");
        return _set_func_auto_spd(p0, p1, p2);
    }

    public static byte @set_func_cmt(void* p0, byte* p1, byte p2)
    {
        if ((nint)_set_func_cmt == 0) throw new System.EntryPointNotFoundException("'set_func_cmt' is not available for the loaded IDA SDK version.");
        return _set_func_cmt(p0, p1, p2);
    }

    public static byte @set_func_cmt_ea(ulong p0, byte* p1, byte p2)
    {
        if ((nint)_set_func_cmt_ea == 0) throw new System.EntryPointNotFoundException("'set_func_cmt_ea' is not available for the loaded IDA SDK version.");
        return _set_func_cmt_ea(p0, p1, p2);
    }

    public static byte @set_func_end(ulong p0, ulong p1)
    {
        if ((nint)_set_func_end == 0) throw new System.EntryPointNotFoundException("'set_func_end' is not available for the loaded IDA SDK version.");
        return _set_func_end(p0, p1);
    }

    public static byte @set_func_entry_info(void* p0)
    {
        if ((nint)_set_func_entry_info == 0) throw new System.EntryPointNotFoundException("'set_func_entry_info' is not available for the loaded IDA SDK version.");
        return _set_func_entry_info(p0);
    }

    public static byte @set_func_flag(ulong p0, ulong p1, byte p2)
    {
        if ((nint)_set_func_flag == 0) throw new System.EntryPointNotFoundException("'set_func_flag' is not available for the loaded IDA SDK version.");
        return _set_func_flag(p0, p1, p2);
    }

    public static byte @set_func_flags(ulong p0, ulong p1)
    {
        if ((nint)_set_func_flags == 0) throw new System.EntryPointNotFoundException("'set_func_flags' is not available for the loaded IDA SDK version.");
        return _set_func_flags(p0, p1);
    }

    public static int @set_func_name_if_jumpfunc(void* p0, byte* p1)
    {
        if ((nint)_set_func_name_if_jumpfunc == 0) throw new System.EntryPointNotFoundException("'set_func_name_if_jumpfunc' is not available for the loaded IDA SDK version.");
        return _set_func_name_if_jumpfunc(p0, p1);
    }

    public static int @set_func_regvar_cmt(ulong p0, nint p1, byte* p2)
    {
        if ((nint)_set_func_regvar_cmt == 0) throw new System.EntryPointNotFoundException("'set_func_regvar_cmt' is not available for the loaded IDA SDK version.");
        return _set_func_regvar_cmt(p0, p1, p2);
    }

    public static int @set_func_regvar_range(ulong p0, nint p1, void* p2)
    {
        if ((nint)_set_func_regvar_range == 0) throw new System.EntryPointNotFoundException("'set_func_regvar_range' is not available for the loaded IDA SDK version.");
        return _set_func_regvar_range(p0, p1, p2);
    }

    public static int @set_func_start(ulong p0, ulong p1)
    {
        if ((nint)_set_func_start == 0) throw new System.EntryPointNotFoundException("'set_func_start' is not available for the loaded IDA SDK version.");
        return _set_func_start(p0, p1);
    }

    public static byte @set_function_name_if_jumpfunc(ulong p0, byte* p1)
    {
        if ((nint)_set_function_name_if_jumpfunc == 0) throw new System.EntryPointNotFoundException("'set_function_name_if_jumpfunc' is not available for the loaded IDA SDK version.");
        return _set_function_name_if_jumpfunc(p0, p1);
    }

    public static int @set_group_selector(ulong p0, ulong p1)
    {
        if ((nint)_set_group_selector == 0) throw new System.EntryPointNotFoundException("'set_group_selector' is not available for the loaded IDA SDK version.");
        return _set_group_selector(p0, p1);
    }

    public static byte @set_header_path(byte* p0, byte p1)
    {
        if ((nint)_set_header_path == 0) throw new System.EntryPointNotFoundException("'set_header_path' is not available for the loaded IDA SDK version.");
        return _set_header_path(p0, p1);
    }

    public static int @set_ida_state(int p0)
    {
        if ((nint)_set_ida_state == 0) throw new System.EntryPointNotFoundException("'set_ida_state' is not available for the loaded IDA SDK version.");
        return _set_ida_state(p0);
    }

    public static byte* @set_idc_dtor(void* p0, byte* p1)
    {
        if ((nint)_set_idc_dtor == 0) throw new System.EntryPointNotFoundException("'set_idc_dtor' is not available for the loaded IDA SDK version.");
        return _set_idc_dtor(p0, p1);
    }

    public static byte* @set_idc_getattr(void* p0, byte* p1)
    {
        if ((nint)_set_idc_getattr == 0) throw new System.EntryPointNotFoundException("'set_idc_getattr' is not available for the loaded IDA SDK version.");
        return _set_idc_getattr(p0, p1);
    }

    public static byte @set_idc_method(void* p0, byte* p1)
    {
        if ((nint)_set_idc_method == 0) throw new System.EntryPointNotFoundException("'set_idc_method' is not available for the loaded IDA SDK version.");
        return _set_idc_method(p0, p1);
    }

    public static byte* @set_idc_setattr(void* p0, byte* p1)
    {
        if ((nint)_set_idc_setattr == 0) throw new System.EntryPointNotFoundException("'set_idc_setattr' is not available for the loaded IDA SDK version.");
        return _set_idc_setattr(p0, p1);
    }

    public static int @set_idcv_attr(void* p0, byte* p1, void* p2, byte p3)
    {
        if ((nint)_set_idcv_attr == 0) throw new System.EntryPointNotFoundException("'set_idcv_attr' is not available for the loaded IDA SDK version.");
        return _set_idcv_attr(p0, p1, p2, p3);
    }

    public static int @set_idcv_slice(void* p0, ulong p1, ulong p2, void* p3, int p4)
    {
        if ((nint)_set_idcv_slice == 0) throw new System.EntryPointNotFoundException("'set_idcv_slice' is not available for the loaded IDA SDK version.");
        return _set_idcv_slice(p0, p1, p2, p3, p4);
    }

    public static byte @set_immd(ulong p0)
    {
        if ((nint)_set_immd == 0) throw new System.EntryPointNotFoundException("'set_immd' is not available for the loaded IDA SDK version.");
        return _set_immd(p0);
    }

    public static void @set_import_name(ulong p0, ulong p1, byte* p2)
    {
        if ((nint)_set_import_name == 0) throw new System.EntryPointNotFoundException("'set_import_name' is not available for the loaded IDA SDK version.");
        _set_import_name(p0, p1, p2);
    }

    public static void @set_import_ordinal(ulong p0, ulong p1, ulong p2)
    {
        if ((nint)_set_import_ordinal == 0) throw new System.EntryPointNotFoundException("'set_import_ordinal' is not available for the loaded IDA SDK version.");
        _set_import_ordinal(p0, p1, p2);
    }

    public static byte @set_interr_throws(byte p0)
    {
        if ((nint)_set_interr_throws == 0) throw new System.EntryPointNotFoundException("'set_interr_throws' is not available for the loaded IDA SDK version.");
        return _set_interr_throws(p0);
    }

    public static void @set_item_color(ulong p0, uint p1)
    {
        if ((nint)_set_item_color == 0) throw new System.EntryPointNotFoundException("'set_item_color' is not available for the loaded IDA SDK version.");
        _set_item_color(p0, p1);
    }

    public static byte @set_lzero(ulong p0, int p1)
    {
        if ((nint)_set_lzero == 0) throw new System.EntryPointNotFoundException("'set_lzero' is not available for the loaded IDA SDK version.");
        return _set_lzero(p0, p1);
    }

    public static void @set_manual_insn(ulong p0, byte* p1)
    {
        if ((nint)_set_manual_insn == 0) throw new System.EntryPointNotFoundException("'set_manual_insn' is not available for the loaded IDA SDK version.");
        _set_manual_insn(p0, p1);
    }

    public static void* @set_module_data(int* p0, void* p1)
    {
        if ((nint)_set_module_data == 0) throw new System.EntryPointNotFoundException("'set_module_data' is not available for the loaded IDA SDK version.");
        return _set_module_data(p0, p1);
    }

    public static byte @set_moved_jpt(void* p0, void* p1, void* p2, void* p3, byte p4, byte p5)
    {
        if ((nint)_set_moved_jpt == 0) throw new System.EntryPointNotFoundException("'set_moved_jpt' is not available for the loaded IDA SDK version.");
        return _set_moved_jpt(p0, p1, p2, p3, p4, p5);
    }

    public static byte @set_name(ulong p0, byte* p1, int p2)
    {
        if ((nint)_set_name == 0) throw new System.EntryPointNotFoundException("'set_name' is not available for the loaded IDA SDK version.");
        return _set_name(p0, p1, p2);
    }

    public static void @set_node_info(ulong p0, int p1, void* p2, uint p3)
    {
        if ((nint)_set_node_info == 0) throw new System.EntryPointNotFoundException("'set_node_info' is not available for the loaded IDA SDK version.");
        _set_node_info(p0, p1, p2, p3);
    }

    public static byte @set_noret_insn(ulong p0, byte p1)
    {
        if ((nint)_set_noret_insn == 0) throw new System.EntryPointNotFoundException("'set_noret_insn' is not available for the loaded IDA SDK version.");
        return _set_noret_insn(p0, p1);
    }

    public static void @set_notcode(ulong p0)
    {
        if ((nint)_set_notcode == 0) throw new System.EntryPointNotFoundException("'set_notcode' is not available for the loaded IDA SDK version.");
        _set_notcode(p0);
    }

    public static byte @set_op_tinfo(ulong p0, int p1, TypeInfo* p2)
    {
        if ((nint)_set_op_tinfo == 0) throw new System.EntryPointNotFoundException("'set_op_tinfo' is not available for the loaded IDA SDK version.");
        return _set_op_tinfo(p0, p1, p2);
    }

    public static byte @set_op_type(ulong p0, ulong p1, int p2)
    {
        if ((nint)_set_op_type == 0) throw new System.EntryPointNotFoundException("'set_op_type' is not available for the loaded IDA SDK version.");
        return _set_op_type(p0, p1, p2);
    }

    public static byte @set_opinfo(ulong p0, int p1, ulong p2, void* p3, byte p4)
    {
        if ((nint)_set_opinfo == 0) throw new System.EntryPointNotFoundException("'set_opinfo' is not available for the loaded IDA SDK version.");
        return _set_opinfo(p0, p1, p2, p3, p4);
    }

    public static byte @set_outfile_encoding_idx(int p0)
    {
        if ((nint)_set_outfile_encoding_idx == 0) throw new System.EntryPointNotFoundException("'set_outfile_encoding_idx' is not available for the loaded IDA SDK version.");
        return _set_outfile_encoding_idx(p0);
    }

    public static int @set_parser_argv(byte* p0, byte* p1)
    {
        if ((nint)_set_parser_argv == 0) throw new System.EntryPointNotFoundException("'set_parser_argv' is not available for the loaded IDA SDK version.");
        return _set_parser_argv(p0, p1);
    }

    public static byte @set_parser_option(byte* p0, byte* p1, byte* p2)
    {
        if ((nint)_set_parser_option == 0) throw new System.EntryPointNotFoundException("'set_parser_option' is not available for the loaded IDA SDK version.");
        return _set_parser_option(p0, p1, p2);
    }

    public static void @set_path(int p0, byte* p1)
    {
        if ((nint)_set_path == 0) throw new System.EntryPointNotFoundException("'set_path' is not available for the loaded IDA SDK version.");
        _set_path(p0, p1);
    }

    public static byte @set_processor_type(byte* p0, int p1)
    {
        if ((nint)_set_processor_type == 0) throw new System.EntryPointNotFoundException("'set_processor_type' is not available for the loaded IDA SDK version.");
        return _set_processor_type(p0, p1);
    }

    public static byte @set_purged(ulong p0, int p1, byte p2)
    {
        if ((nint)_set_purged == 0) throw new System.EntryPointNotFoundException("'set_purged' is not available for the loaded IDA SDK version.");
        return _set_purged(p0, p1, p2);
    }

    public static int @set_qerrno(int p0)
    {
        if ((nint)_set_qerrno == 0) throw new System.EntryPointNotFoundException("'set_qerrno' is not available for the loaded IDA SDK version.");
        return _set_qerrno(p0);
    }

    public static byte @set_refinfo(ulong p0, int p1, byte p2, ulong p3, ulong p4, long p5)
    {
        if ((nint)_set_refinfo == 0) throw new System.EntryPointNotFoundException("'set_refinfo' is not available for the loaded IDA SDK version.");
        return _set_refinfo(p0, p1, p2, p3, p4, p5);
    }

    public static byte @set_refinfo_ex(ulong p0, int p1, void* p2)
    {
        if ((nint)_set_refinfo_ex == 0) throw new System.EntryPointNotFoundException("'set_refinfo_ex' is not available for the loaded IDA SDK version.");
        return _set_refinfo_ex(p0, p1, p2);
    }

    public static byte @set_registry_name(byte* p0)
    {
        if ((nint)_set_registry_name == 0) throw new System.EntryPointNotFoundException("'set_registry_name' is not available for the loaded IDA SDK version.");
        return _set_registry_name(p0);
    }

    public static int @set_regvar_cmt(void* p0, void* p1, byte* p2)
    {
        if ((nint)_set_regvar_cmt == 0) throw new System.EntryPointNotFoundException("'set_regvar_cmt' is not available for the loaded IDA SDK version.");
        return _set_regvar_cmt(p0, p1, p2);
    }

    public static void @set_screen_ea(ulong p0)
    {
        if ((nint)_set_screen_ea == 0) throw new System.EntryPointNotFoundException("'set_screen_ea' is not available for the loaded IDA SDK version.");
        _set_screen_ea(p0);
    }

    public static byte @set_segm_addressing(void* p0, nuint p1)
    {
        if ((nint)_set_segm_addressing == 0) throw new System.EntryPointNotFoundException("'set_segm_addressing' is not available for the loaded IDA SDK version.");
        return _set_segm_addressing(p0, p1);
    }

    public static byte @set_segm_base(void* p0, ulong p1)
    {
        if ((nint)_set_segm_base == 0) throw new System.EntryPointNotFoundException("'set_segm_base' is not available for the loaded IDA SDK version.");
        return _set_segm_base(p0, p1);
    }

    public static int @set_segm_class(void* p0, byte* p1, int p2)
    {
        if ((nint)_set_segm_class == 0) throw new System.EntryPointNotFoundException("'set_segm_class' is not available for the loaded IDA SDK version.");
        return _set_segm_class(p0, p1, p2);
    }

    public static byte @set_segm_end(ulong p0, ulong p1, int p2)
    {
        if ((nint)_set_segm_end == 0) throw new System.EntryPointNotFoundException("'set_segm_end' is not available for the loaded IDA SDK version.");
        return _set_segm_end(p0, p1, p2);
    }

    public static int @set_segm_name(void* p0, byte* p1, int p2)
    {
        if ((nint)_set_segm_name == 0) throw new System.EntryPointNotFoundException("'set_segm_name' is not available for the loaded IDA SDK version.");
        return _set_segm_name(p0, p1, p2);
    }

    public static byte @set_segm_start(ulong p0, ulong p1, int p2)
    {
        if ((nint)_set_segm_start == 0) throw new System.EntryPointNotFoundException("'set_segm_start' is not available for the loaded IDA SDK version.");
        return _set_segm_start(p0, p1, p2);
    }

    public static byte @set_segment_addressing(ulong p0, nuint p1)
    {
        if ((nint)_set_segment_addressing == 0) throw new System.EntryPointNotFoundException("'set_segment_addressing' is not available for the loaded IDA SDK version.");
        return _set_segment_addressing(p0, p1);
    }

    public static byte @set_segment_base_ea(ulong p0, ulong p1)
    {
        if ((nint)_set_segment_base_ea == 0) throw new System.EntryPointNotFoundException("'set_segment_base_ea' is not available for the loaded IDA SDK version.");
        return _set_segment_base_ea(p0, p1);
    }

    public static int @set_segment_class(ulong p0, byte* p1, int p2)
    {
        if ((nint)_set_segment_class == 0) throw new System.EntryPointNotFoundException("'set_segment_class' is not available for the loaded IDA SDK version.");
        return _set_segment_class(p0, p1, p2);
    }

    public static void @set_segment_cmt(void* p0, byte* p1, byte p2)
    {
        if ((nint)_set_segment_cmt == 0) throw new System.EntryPointNotFoundException("'set_segment_cmt' is not available for the loaded IDA SDK version.");
        _set_segment_cmt(p0, p1, p2);
    }

    public static void @set_segment_cmt_by_ea(ulong p0, byte* p1, byte p2)
    {
        if ((nint)_set_segment_cmt_by_ea == 0) throw new System.EntryPointNotFoundException("'set_segment_cmt_by_ea' is not available for the loaded IDA SDK version.");
        _set_segment_cmt_by_ea(p0, p1, p2);
    }

    public static byte @set_segment_info(void* p0, int p1)
    {
        if ((nint)_set_segment_info == 0) throw new System.EntryPointNotFoundException("'set_segment_info' is not available for the loaded IDA SDK version.");
        return _set_segment_info(p0, p1);
    }

    public static int @set_segment_name(ulong p0, byte* p1, int p2)
    {
        if ((nint)_set_segment_name == 0) throw new System.EntryPointNotFoundException("'set_segment_name' is not available for the loaded IDA SDK version.");
        return _set_segment_name(p0, p1, p2);
    }

    public static byte @set_segment_translations(ulong p0, void* p1)
    {
        if ((nint)_set_segment_translations == 0) throw new System.EntryPointNotFoundException("'set_segment_translations' is not available for the loaded IDA SDK version.");
        return _set_segment_translations(p0, p1);
    }

    public static int @set_selector(ulong p0, ulong p1)
    {
        if ((nint)_set_selector == 0) throw new System.EntryPointNotFoundException("'set_selector' is not available for the loaded IDA SDK version.");
        return _set_selector(p0, p1);
    }

    public static void @set_source_linnum(ulong p0, ulong p1)
    {
        if ((nint)_set_source_linnum == 0) throw new System.EntryPointNotFoundException("'set_source_linnum' is not available for the loaded IDA SDK version.");
        _set_source_linnum(p0, p1);
    }

    public static void @set_sreg_at_next_code(ulong p0, ulong p1, int p2, ulong p3)
    {
        if ((nint)_set_sreg_at_next_code == 0) throw new System.EntryPointNotFoundException("'set_sreg_at_next_code' is not available for the loaded IDA SDK version.");
        _set_sreg_at_next_code(p0, p1, p2, p3);
    }

    public static void @set_str_type(ulong p0, uint p1)
    {
        if ((nint)_set_str_type == 0) throw new System.EntryPointNotFoundException("'set_str_type' is not available for the loaded IDA SDK version.");
        _set_str_type(p0, p1);
    }

    public static void @set_switch_info(ulong p0, void* p1)
    {
        if ((nint)_set_switch_info == 0) throw new System.EntryPointNotFoundException("'set_switch_info' is not available for the loaded IDA SDK version.");
        _set_switch_info(p0, p1);
    }

    public static byte @set_tail_owner(void* p0, ulong p1)
    {
        if ((nint)_set_tail_owner == 0) throw new System.EntryPointNotFoundException("'set_tail_owner' is not available for the loaded IDA SDK version.");
        return _set_tail_owner(p0, p1);
    }

    public static byte @set_tail_owner_ea(ulong p0, ulong p1)
    {
        if ((nint)_set_tail_owner_ea == 0) throw new System.EntryPointNotFoundException("'set_tail_owner_ea' is not available for the loaded IDA SDK version.");
        return _set_tail_owner_ea(p0, p1);
    }

    public static byte @set_target_assembler(int p0)
    {
        if ((nint)_set_target_assembler == 0) throw new System.EntryPointNotFoundException("'set_target_assembler' is not available for the loaded IDA SDK version.");
        return _set_target_assembler(p0);
    }

    public static byte @set_tinfo(ulong p0, TypeInfo* p1)
    {
        if ((nint)_set_tinfo == 0) throw new System.EntryPointNotFoundException("'set_tinfo' is not available for the loaded IDA SDK version.");
        return _set_tinfo(p0, p1);
    }

    public static byte @set_tinfo_attr(TypeInfo* p0, void* p1, byte p2)
    {
        if ((nint)_set_tinfo_attr == 0) throw new System.EntryPointNotFoundException("'set_tinfo_attr' is not available for the loaded IDA SDK version.");
        return _set_tinfo_attr(p0, p1, p2);
    }

    public static byte @set_tinfo_attrs(TypeInfo* p0, void* p1)
    {
        if ((nint)_set_tinfo_attrs == 0) throw new System.EntryPointNotFoundException("'set_tinfo_attrs' is not available for the loaded IDA SDK version.");
        return _set_tinfo_attrs(p0, p1);
    }

    public static nuint @set_tinfo_property(TypeInfo* p0, int p1, nuint p2)
    {
        if ((nint)_set_tinfo_property == 0) throw new System.EntryPointNotFoundException("'set_tinfo_property' is not available for the loaded IDA SDK version.");
        return _set_tinfo_property(p0, p1, p2);
    }

    public static nuint @set_tinfo_property4(TypeInfo* p0, int p1, nuint p2, nuint p3, nuint p4, nuint p5)
    {
        if ((nint)_set_tinfo_property4 == 0) throw new System.EntryPointNotFoundException("'set_tinfo_property4' is not available for the loaded IDA SDK version.");
        return _set_tinfo_property4(p0, p1, p2, p3, p4, p5);
    }

    public static byte @set_type_alias(void* p0, uint p1, uint p2)
    {
        if ((nint)_set_type_alias == 0) throw new System.EntryPointNotFoundException("'set_type_alias' is not available for the loaded IDA SDK version.");
        return _set_type_alias(p0, p1, p2);
    }

    public static void @set_type_choosable(void* p0, uint p1, byte p2)
    {
        if ((nint)_set_type_choosable == 0) throw new System.EntryPointNotFoundException("'set_type_choosable' is not available for the loaded IDA SDK version.");
        _set_type_choosable(p0, p1, p2);
    }

    public static byte @set_vftable_ea(uint p0, ulong p1)
    {
        if ((nint)_set_vftable_ea == 0) throw new System.EntryPointNotFoundException("'set_vftable_ea' is not available for the loaded IDA SDK version.");
        return _set_vftable_ea(p0, p1);
    }

    public static void @set_visible_func(void* p0, byte p1)
    {
        if ((nint)_set_visible_func == 0) throw new System.EntryPointNotFoundException("'set_visible_func' is not available for the loaded IDA SDK version.");
        _set_visible_func(p0, p1);
    }

    public static void @set_visible_func_ea(ulong p0, byte p1)
    {
        if ((nint)_set_visible_func_ea == 0) throw new System.EntryPointNotFoundException("'set_visible_func_ea' is not available for the loaded IDA SDK version.");
        _set_visible_func_ea(p0, p1);
    }

    public static void @set_visible_segm(void* p0, byte p1)
    {
        if ((nint)_set_visible_segm == 0) throw new System.EntryPointNotFoundException("'set_visible_segm' is not available for the loaded IDA SDK version.");
        _set_visible_segm(p0, p1);
    }

    public static void @set_visible_segment(ulong p0, byte p1)
    {
        if ((nint)_set_visible_segment == 0) throw new System.EntryPointNotFoundException("'set_visible_segment' is not available for the loaded IDA SDK version.");
        _set_visible_segment(p0, p1);
    }

    public static void @set_xrefpos(ulong p0, void* p1)
    {
        if ((nint)_set_xrefpos == 0) throw new System.EntryPointNotFoundException("'set_xrefpos' is not available for the loaded IDA SDK version.");
        _set_xrefpos(p0, p1);
    }

    public static byte @setinf(int p0, nint p1)
    {
        if ((nint)_setinf == 0) throw new System.EntryPointNotFoundException("'setinf' is not available for the loaded IDA SDK version.");
        return _setinf(p0, p1);
    }

    public static byte @setinf_buf(int p0, void* p1, nuint p2)
    {
        if ((nint)_setinf_buf == 0) throw new System.EntryPointNotFoundException("'setinf_buf' is not available for the loaded IDA SDK version.");
        return _setinf_buf(p0, p1, p2);
    }

    public static byte @setinf_flag(int p0, uint p1, byte p2)
    {
        if ((nint)_setinf_flag == 0) throw new System.EntryPointNotFoundException("'setinf_flag' is not available for the loaded IDA SDK version.");
        return _setinf_flag(p0, p1, p2);
    }

    public static void @setup_lowcnd_regfuncs(void* p0, void* p1)
    {
        if ((nint)_setup_lowcnd_regfuncs == 0) throw new System.EntryPointNotFoundException("'setup_lowcnd_regfuncs' is not available for the loaded IDA SDK version.");
        _setup_lowcnd_regfuncs(p0, p1);
    }

    public static ulong @setup_selector(ulong p0)
    {
        if ((nint)_setup_selector == 0) throw new System.EntryPointNotFoundException("'setup_selector' is not available for the loaded IDA SDK version.");
        return _setup_selector(p0);
    }

    public static void @show_auto(ulong p0, int p1)
    {
        if ((nint)_show_auto == 0) throw new System.EntryPointNotFoundException("'show_auto' is not available for the loaded IDA SDK version.");
        _show_auto(p0, p1);
    }

    public static void @show_name(ulong p0)
    {
        if ((nint)_show_name == 0) throw new System.EntryPointNotFoundException("'show_name' is not available for the loaded IDA SDK version.");
        _show_name(p0);
    }

    public static byte* @skip_spaces(byte* p0)
    {
        if ((nint)_skip_spaces == 0) throw new System.EntryPointNotFoundException("'skip_spaces' is not available for the loaded IDA SDK version.");
        return _skip_spaces(p0);
    }

    public static nuint @skip_utf8(byte** p0, nuint p1)
    {
        if ((nint)_skip_utf8 == 0) throw new System.EntryPointNotFoundException("'skip_utf8' is not available for the loaded IDA SDK version.");
        return _skip_utf8(p0, p1);
    }

    public static long @soff_to_fpoff_ea(ulong p0, ulong p1)
    {
        if ((nint)_soff_to_fpoff_ea == 0) throw new System.EntryPointNotFoundException("'soff_to_fpoff_ea' is not available for the loaded IDA SDK version.");
        return _soff_to_fpoff_ea(p0, p1);
    }

    public static byte @sort_til(void* p0)
    {
        if ((nint)_sort_til == 0) throw new System.EntryPointNotFoundException("'sort_til' is not available for the loaded IDA SDK version.");
        return _sort_til(p0);
    }

    public static byte @split_sreg_range(ulong p0, int p1, ulong p2, byte p3, byte p4)
    {
        if ((nint)_split_sreg_range == 0) throw new System.EntryPointNotFoundException("'split_sreg_range' is not available for the loaded IDA SDK version.");
        return _split_sreg_range(p0, p1, p2, p3, p4);
    }

    public static void @std_out_segm_footer(void* p0, void* p1)
    {
        if ((nint)_std_out_segm_footer == 0) throw new System.EntryPointNotFoundException("'std_out_segm_footer' is not available for the loaded IDA SDK version.");
        _std_out_segm_footer(p0, p1);
    }

    public static void @std_out_segment_footer(void* p0, ulong p1)
    {
        if ((nint)_std_out_segment_footer == 0) throw new System.EntryPointNotFoundException("'std_out_segment_footer' is not available for the loaded IDA SDK version.");
        _std_out_segment_footer(p0, p1);
    }

    public static nuint @stoa(QString* p0, ulong p1, ulong p2)
    {
        if ((nint)_stoa == 0) throw new System.EntryPointNotFoundException("'stoa' is not available for the loaded IDA SDK version.");
        return _stoa(p0, p1, p2);
    }

    public static byte @store_til(void* p0, byte* p1, byte* p2)
    {
        if ((nint)_store_til == 0) throw new System.EntryPointNotFoundException("'store_til' is not available for the loaded IDA SDK version.");
        return _store_til(p0, p1, p2);
    }

    public static byte @str2ea(ulong* p0, byte* p1, ulong p2)
    {
        if ((nint)_str2ea == 0) throw new System.EntryPointNotFoundException("'str2ea' is not available for the loaded IDA SDK version.");
        return _str2ea(p0, p1, p2);
    }

    public static byte @str2ea_ex(ulong* p0, byte* p1, ulong p2, int p3)
    {
        if ((nint)_str2ea_ex == 0) throw new System.EntryPointNotFoundException("'str2ea_ex' is not available for the loaded IDA SDK version.");
        return _str2ea_ex(p0, p1, p2, p3);
    }

    public static int @str2reg(byte* p0)
    {
        if ((nint)_str2reg == 0) throw new System.EntryPointNotFoundException("'str2reg' is not available for the loaded IDA SDK version.");
        return _str2reg(p0);
    }

    public static byte* @str2user(byte* p0, byte* p1, nuint p2)
    {
        if ((nint)_str2user == 0) throw new System.EntryPointNotFoundException("'str2user' is not available for the loaded IDA SDK version.");
        return _str2user(p0, p1, p2);
    }

    public static byte* @strarray(void* p0, nuint p1, int p2)
    {
        if ((nint)_strarray == 0) throw new System.EntryPointNotFoundException("'strarray' is not available for the loaded IDA SDK version.");
        return _strarray(p0, p1, p2);
    }

    public static void @strdiff_t_serialize(void* p0, void* p1)
    {
        if ((nint)_strdiff_t_serialize == 0) throw new System.EntryPointNotFoundException("'strdiff_t_serialize' is not available for the loaded IDA SDK version.");
        _strdiff_t_serialize(p0, p1);
    }

    public static byte* @stristr(byte* p0, byte* p1)
    {
        if ((nint)_stristr == 0) throw new System.EntryPointNotFoundException("'stristr' is not available for the loaded IDA SDK version.");
        return _stristr(p0, p1);
    }

    public static byte* @strrpl(byte* p0, int p1, int p2)
    {
        if ((nint)_strrpl == 0) throw new System.EntryPointNotFoundException("'strrpl' is not available for the loaded IDA SDK version.");
        return _strrpl(p0, p1, p2);
    }

    public static void @swap128(void* p0)
    {
        if ((nint)_swap128 == 0) throw new System.EntryPointNotFoundException("'swap128' is not available for the loaded IDA SDK version.");
        _swap128(p0);
    }

    public static ulong @swap64(ulong p0)
    {
        if ((nint)_swap64 == 0) throw new System.EntryPointNotFoundException("'swap64' is not available for the loaded IDA SDK version.");
        return _swap64(p0);
    }

    public static void @swap_idcvs(void* p0, void* p1)
    {
        if ((nint)_swap_idcvs == 0) throw new System.EntryPointNotFoundException("'swap_idcvs' is not available for the loaded IDA SDK version.");
        _swap_idcvs(p0, p1);
    }

    public static void @swap_value(void* p0, void* p1, int p2)
    {
        if ((nint)_swap_value == 0) throw new System.EntryPointNotFoundException("'swap_value' is not available for the loaded IDA SDK version.");
        _swap_value(p0, p1, p2);
    }

    public static void* @switch_dbctx(nuint p0)
    {
        if ((nint)_switch_dbctx == 0) throw new System.EntryPointNotFoundException("'switch_dbctx' is not available for the loaded IDA SDK version.");
        return _switch_dbctx(p0);
    }

    public static void @switch_to_rust()
    {
        if ((nint)_switch_to_rust == 0) throw new System.EntryPointNotFoundException("'switch_to_rust' is not available for the loaded IDA SDK version.");
        _switch_to_rust();
    }

    public static void @tag_addr(QString* p0, ulong p1, byte p2)
    {
        if ((nint)_tag_addr == 0) throw new System.EntryPointNotFoundException("'tag_addr' is not available for the loaded IDA SDK version.");
        _tag_addr(p0, p1, p2);
    }

    public static byte* @tag_advance(byte* p0, int p1)
    {
        if ((nint)_tag_advance == 0) throw new System.EntryPointNotFoundException("'tag_advance' is not available for the loaded IDA SDK version.");
        return _tag_advance(p0, p1);
    }

    public static ulong @tag_get_addr(byte* p0)
    {
        if ((nint)_tag_get_addr == 0) throw new System.EntryPointNotFoundException("'tag_get_addr' is not available for the loaded IDA SDK version.");
        return _tag_get_addr(p0);
    }

    public static nint @tag_remove(QString* p0, byte* p1, int p2)
    {
        if ((nint)_tag_remove == 0) throw new System.EntryPointNotFoundException("'tag_remove' is not available for the loaded IDA SDK version.");
        return _tag_remove(p0, p1, p2);
    }

    public static byte* @tag_skipcode(byte* p0)
    {
        if ((nint)_tag_skipcode == 0) throw new System.EntryPointNotFoundException("'tag_skipcode' is not available for the loaded IDA SDK version.");
        return _tag_skipcode(p0);
    }

    public static byte* @tag_skipcodes(byte* p0)
    {
        if ((nint)_tag_skipcodes == 0) throw new System.EntryPointNotFoundException("'tag_skipcodes' is not available for the loaded IDA SDK version.");
        return _tag_skipcodes(p0);
    }

    public static nint @tag_strlen(byte* p0)
    {
        if ((nint)_tag_strlen == 0) throw new System.EntryPointNotFoundException("'tag_strlen' is not available for the loaded IDA SDK version.");
        return _tag_strlen(p0);
    }

    public static ulong @tagged_line_section_t_get_addr(void* p0, QString* p1)
    {
        if ((nint)_tagged_line_section_t_get_addr == 0) throw new System.EntryPointNotFoundException("'tagged_line_section_t_get_addr' is not available for the loaded IDA SDK version.");
        return _tagged_line_section_t_get_addr(p0, p1);
    }

    public static byte @take_memory_snapshot(int p0)
    {
        if ((nint)_take_memory_snapshot == 0) throw new System.EntryPointNotFoundException("'take_memory_snapshot' is not available for the loaded IDA SDK version.");
        return _take_memory_snapshot(p0);
    }

    public static void @term_database()
    {
        if ((nint)_term_database == 0) throw new System.EntryPointNotFoundException("'term_database' is not available for the loaded IDA SDK version.");
        _term_database();
    }

    public static void @term_plugins(int p0)
    {
        if ((nint)_term_plugins == 0) throw new System.EntryPointNotFoundException("'term_plugins' is not available for the loaded IDA SDK version.");
        _term_plugins(p0);
    }

    public static int @term_process(void* p0)
    {
        if ((nint)_term_process == 0) throw new System.EntryPointNotFoundException("'term_process' is not available for the loaded IDA SDK version.");
        return _term_process(p0);
    }

    public static int @throw_idc_exception(void* p0, byte* p1)
    {
        if ((nint)_throw_idc_exception == 0) throw new System.EntryPointNotFoundException("'throw_idc_exception' is not available for the loaded IDA SDK version.");
        return _throw_idc_exception(p0, p1);
    }

    public static byte* @tinfo_errstr(int p0)
    {
        if ((nint)_tinfo_errstr == 0) throw new System.EntryPointNotFoundException("'tinfo_errstr' is not available for the loaded IDA SDK version.");
        return _tinfo_errstr(p0);
    }

    public static byte @tinfo_get_func_frame(TypeInfo* p0, void* p1)
    {
        if ((nint)_tinfo_get_func_frame == 0) throw new System.EntryPointNotFoundException("'tinfo_get_func_frame' is not available for the loaded IDA SDK version.");
        return _tinfo_get_func_frame(p0, p1);
    }

    public static byte @tinfo_get_func_frame_ea(TypeInfo* p0, ulong p1)
    {
        if ((nint)_tinfo_get_func_frame_ea == 0) throw new System.EntryPointNotFoundException("'tinfo_get_func_frame_ea' is not available for the loaded IDA SDK version.");
        return _tinfo_get_func_frame_ea(p0, p1);
    }

    public static void @tinfo_get_innermost_udm(TypeInfo* p0, TypeInfo* p1, ulong p2, nuint* p3, ulong* p4, byte p5)
    {
        if ((nint)_tinfo_get_innermost_udm == 0) throw new System.EntryPointNotFoundException("'tinfo_get_innermost_udm' is not available for the loaded IDA SDK version.");
        _tinfo_get_innermost_udm(p0, p1, p2, p3, p4, p5);
    }

    public static byte @tinfo_t__build_anon_type_name(QString* p0, TypeInfo* p1)
    {
        if ((nint)_tinfo_t__build_anon_type_name == 0) throw new System.EntryPointNotFoundException("'tinfo_t__build_anon_type_name' is not available for the loaded IDA SDK version.");
        return _tinfo_t__build_anon_type_name(p0, p1);
    }

    public static byte @toggle_bnot(ulong p0, int p1)
    {
        if ((nint)_toggle_bnot == 0) throw new System.EntryPointNotFoundException("'toggle_bnot' is not available for the loaded IDA SDK version.");
        return _toggle_bnot(p0, p1);
    }

    public static byte @toggle_sign(ulong p0, int p1)
    {
        if ((nint)_toggle_sign == 0) throw new System.EntryPointNotFoundException("'toggle_sign' is not available for the loaded IDA SDK version.");
        return _toggle_sign(p0, p1);
    }

    public static byte @track_value_until_address_jpt(void* p0, void* p1, ulong p2)
    {
        if ((nint)_track_value_until_address_jpt == 0) throw new System.EntryPointNotFoundException("'track_value_until_address_jpt' is not available for the loaded IDA SDK version.");
        return _track_value_until_address_jpt(p0, p1, p2);
    }

    public static byte* @trim(byte* p0)
    {
        if ((nint)_trim == 0) throw new System.EntryPointNotFoundException("'trim' is not available for the loaded IDA SDK version.");
        return _trim(p0);
    }

    public static void @trim_jtable(void* p0, ulong p1, byte p2)
    {
        if ((nint)_trim_jtable == 0) throw new System.EntryPointNotFoundException("'trim_jtable' is not available for the loaded IDA SDK version.");
        _trim_jtable(p0, p1, p2);
    }

    public static int @try_to_add_libfunc(ulong p0)
    {
        if ((nint)_try_to_add_libfunc == 0) throw new System.EntryPointNotFoundException("'try_to_add_libfunc' is not available for the loaded IDA SDK version.");
        return _try_to_add_libfunc(p0);
    }

    public static void @txtdiff_t_diff_mod(void* p0)
    {
        if ((nint)_txtdiff_t_diff_mod == 0) throw new System.EntryPointNotFoundException("'txtdiff_t_diff_mod' is not available for the loaded IDA SDK version.");
        _txtdiff_t_diff_mod(p0);
    }

    public static void @txtdiff_t_serialize(void* p0, void* p1)
    {
        if ((nint)_txtdiff_t_serialize == 0) throw new System.EntryPointNotFoundException("'txtdiff_t_serialize' is not available for the loaded IDA SDK version.");
        _txtdiff_t_serialize(p0, p1);
    }

    public static byte @udm_t__compare_with(void* p0, void* p1, int p2)
    {
        if ((nint)_udm_t__compare_with == 0) throw new System.EntryPointNotFoundException("'udm_t__compare_with' is not available for the loaded IDA SDK version.");
        return _udm_t__compare_with(p0, p1, p2);
    }

    public static byte @udt_type_data_t__deduplicate_members(void* p0)
    {
        if ((nint)_udt_type_data_t__deduplicate_members == 0) throw new System.EntryPointNotFoundException("'udt_type_data_t__deduplicate_members' is not available for the loaded IDA SDK version.");
        return _udt_type_data_t__deduplicate_members(p0);
    }

    public static nint @udt_type_data_t__find_member(void* p0, void* p1, int p2)
    {
        if ((nint)_udt_type_data_t__find_member == 0) throw new System.EntryPointNotFoundException("'udt_type_data_t__find_member' is not available for the loaded IDA SDK version.");
        return _udt_type_data_t__find_member(p0, p1, p2);
    }

    public static nint @udt_type_data_t__get_best_fit_member(void* p0, ulong p1)
    {
        if ((nint)_udt_type_data_t__get_best_fit_member == 0) throw new System.EntryPointNotFoundException("'udt_type_data_t__get_best_fit_member' is not available for the loaded IDA SDK version.");
        return _udt_type_data_t__get_best_fit_member(p0, p1);
    }

    public static byte @unhook_event_listener(int p0, void* p1)
    {
        if ((nint)_unhook_event_listener == 0) throw new System.EntryPointNotFoundException("'unhook_event_listener' is not available for the loaded IDA SDK version.");
        return _unhook_event_listener(p0, p1);
    }

    public static int @unhook_from_notification_point(int p0, void* p1, void* p2)
    {
        if ((nint)_unhook_from_notification_point == 0) throw new System.EntryPointNotFoundException("'unhook_from_notification_point' is not available for the loaded IDA SDK version.");
        return _unhook_from_notification_point(p0, p1, p2);
    }

    public static void @unlock_dbgmem_config()
    {
        if ((nint)_unlock_dbgmem_config == 0) throw new System.EntryPointNotFoundException("'unlock_dbgmem_config' is not available for the loaded IDA SDK version.");
        _unlock_dbgmem_config();
    }

    public static void @unmake_linput(void* p0)
    {
        if ((nint)_unmake_linput == 0) throw new System.EntryPointNotFoundException("'unmake_linput' is not available for the loaded IDA SDK version.");
        _unmake_linput(p0);
    }

    public static uint @unpack_dd(byte** p0, byte* p1)
    {
        if ((nint)_unpack_dd == 0) throw new System.EntryPointNotFoundException("'unpack_dd' is not available for the loaded IDA SDK version.");
        return _unpack_dd(p0, p1);
    }

    public static ulong @unpack_dq(byte** p0, byte* p1)
    {
        if ((nint)_unpack_dq == 0) throw new System.EntryPointNotFoundException("'unpack_dq' is not available for the loaded IDA SDK version.");
        return _unpack_dq(p0, p1);
    }

    public static byte* @unpack_ds(byte** p0, byte* p1, byte p2)
    {
        if ((nint)_unpack_ds == 0) throw new System.EntryPointNotFoundException("'unpack_ds' is not available for the loaded IDA SDK version.");
        return _unpack_ds(p0, p1, p2);
    }

    public static ushort @unpack_dw(byte** p0, byte* p1)
    {
        if ((nint)_unpack_dw == 0) throw new System.EntryPointNotFoundException("'unpack_dw' is not available for the loaded IDA SDK version.");
        return _unpack_dw(p0, p1);
    }

    public static int @unpack_idcobj_from_bv(void* p0, TypeInfo* p1, void* p2, int p3)
    {
        if ((nint)_unpack_idcobj_from_bv == 0) throw new System.EntryPointNotFoundException("'unpack_idcobj_from_bv' is not available for the loaded IDA SDK version.");
        return _unpack_idcobj_from_bv(p0, p1, p2, p3);
    }

    public static int @unpack_idcobj_from_idb(void* p0, TypeInfo* p1, ulong p2, void* p3, int p4)
    {
        if ((nint)_unpack_idcobj_from_idb == 0) throw new System.EntryPointNotFoundException("'unpack_idcobj_from_idb' is not available for the loaded IDA SDK version.");
        return _unpack_idcobj_from_idb(p0, p1, p2, p3, p4);
    }

    public static byte @unpack_xleb128(void* p0, int p1, byte p2, byte** p3, byte* p4)
    {
        if ((nint)_unpack_xleb128 == 0) throw new System.EntryPointNotFoundException("'unpack_xleb128' is not available for the loaded IDA SDK version.");
        return _unpack_xleb128(p0, p1, p2, p3, p4);
    }

    public static byte @unregister_custom_callcnv(uint p0)
    {
        if ((nint)_unregister_custom_callcnv == 0) throw new System.EntryPointNotFoundException("'unregister_custom_callcnv' is not available for the loaded IDA SDK version.");
        return _unregister_custom_callcnv(p0);
    }

    public static byte @unregister_custom_data_format(int p0)
    {
        if ((nint)_unregister_custom_data_format == 0) throw new System.EntryPointNotFoundException("'unregister_custom_data_format' is not available for the loaded IDA SDK version.");
        return _unregister_custom_data_format(p0);
    }

    public static byte @unregister_custom_data_type(int p0)
    {
        if ((nint)_unregister_custom_data_type == 0) throw new System.EntryPointNotFoundException("'unregister_custom_data_type' is not available for the loaded IDA SDK version.");
        return _unregister_custom_data_type(p0);
    }

    public static byte @unregister_custom_fixup(ushort p0)
    {
        if ((nint)_unregister_custom_fixup == 0) throw new System.EntryPointNotFoundException("'unregister_custom_fixup' is not available for the loaded IDA SDK version.");
        return _unregister_custom_fixup(p0);
    }

    public static byte @unregister_custom_refinfo(int p0)
    {
        if ((nint)_unregister_custom_refinfo == 0) throw new System.EntryPointNotFoundException("'unregister_custom_refinfo' is not available for the loaded IDA SDK version.");
        return _unregister_custom_refinfo(p0);
    }

    public static byte @unregister_post_event_visitor(int p0, void* p1)
    {
        if ((nint)_unregister_post_event_visitor == 0) throw new System.EntryPointNotFoundException("'unregister_post_event_visitor' is not available for the loaded IDA SDK version.");
        return _unregister_post_event_visitor(p0, p1);
    }

    public static void @upd_abits(ulong p0, uint p1, uint p2)
    {
        if ((nint)_upd_abits == 0) throw new System.EntryPointNotFoundException("'upd_abits' is not available for the loaded IDA SDK version.");
        _upd_abits(p0, p1, p2);
    }

    public static byte @update_extra_cmt(ulong p0, int p1, byte* p2)
    {
        if ((nint)_update_extra_cmt == 0) throw new System.EntryPointNotFoundException("'update_extra_cmt' is not available for the loaded IDA SDK version.");
        return _update_extra_cmt(p0, p1, p2);
    }

    public static byte @update_fpd(void* p0, ulong p1)
    {
        if ((nint)_update_fpd == 0) throw new System.EntryPointNotFoundException("'update_fpd' is not available for the loaded IDA SDK version.");
        return _update_fpd(p0, p1);
    }

    public static byte @update_fpd_ea(ulong p0, ulong p1)
    {
        if ((nint)_update_fpd_ea == 0) throw new System.EntryPointNotFoundException("'update_fpd_ea' is not available for the loaded IDA SDK version.");
        return _update_fpd_ea(p0, p1);
    }

    public static byte @update_func(void* p0)
    {
        if ((nint)_update_func == 0) throw new System.EntryPointNotFoundException("'update_func' is not available for the loaded IDA SDK version.");
        return _update_func(p0);
    }

    public static byte @update_hidden_range(void* p0)
    {
        if ((nint)_update_hidden_range == 0) throw new System.EntryPointNotFoundException("'update_hidden_range' is not available for the loaded IDA SDK version.");
        return _update_hidden_range(p0);
    }

    public static byte @update_hidden_range_info(void* p0)
    {
        if ((nint)_update_hidden_range_info == 0) throw new System.EntryPointNotFoundException("'update_hidden_range_info' is not available for the loaded IDA SDK version.");
        return _update_hidden_range_info(p0);
    }

    public static byte @update_segm(void* p0)
    {
        if ((nint)_update_segm == 0) throw new System.EntryPointNotFoundException("'update_segm' is not available for the loaded IDA SDK version.");
        return _update_segm(p0);
    }

    public static byte @update_snapshot_attributes(byte* p0, void* p1, void* p2, int p3)
    {
        if ((nint)_update_snapshot_attributes == 0) throw new System.EntryPointNotFoundException("'update_snapshot_attributes' is not available for the loaded IDA SDK version.");
        return _update_snapshot_attributes(p0, p1, p2, p3);
    }

    public static ulong @use_mapping(ulong p0)
    {
        if ((nint)_use_mapping == 0) throw new System.EntryPointNotFoundException("'use_mapping' is not available for the loaded IDA SDK version.");
        return _use_mapping(p0);
    }

    public static void @user2qstr(QString* p0, QString* p1)
    {
        if ((nint)_user2qstr == 0) throw new System.EntryPointNotFoundException("'user2qstr' is not available for the loaded IDA SDK version.");
        _user2qstr(p0, p1);
    }

    public static byte* @user2str(byte* p0, byte* p1, nuint p2)
    {
        if ((nint)_user2str == 0) throw new System.EntryPointNotFoundException("'user2str' is not available for the loaded IDA SDK version.");
        return _user2str(p0, p1, p2);
    }

    public static byte @utf16_utf8(QString* p0, void* p1, int p2)
    {
        if ((nint)_utf16_utf8 == 0) throw new System.EntryPointNotFoundException("'utf16_utf8' is not available for the loaded IDA SDK version.");
        return _utf16_utf8(p0, p1, p2);
    }

    public static byte @utf8_utf16(void* p0, byte* p1, int p2)
    {
        if ((nint)_utf8_utf16 == 0) throw new System.EntryPointNotFoundException("'utf8_utf16' is not available for the loaded IDA SDK version.");
        return _utf8_utf16(p0, p1, p2);
    }

    public static nuint @validate_idb(uint p0)
    {
        if ((nint)_validate_idb == 0) throw new System.EntryPointNotFoundException("'validate_idb' is not available for the loaded IDA SDK version.");
        return _validate_idb(p0);
    }

    public static int @validate_idb_names(byte p0)
    {
        if ((nint)_validate_idb_names == 0) throw new System.EntryPointNotFoundException("'validate_idb_names' is not available for the loaded IDA SDK version.");
        return _validate_idb_names(p0);
    }

    public static byte @validate_name(QString* p0, int p1, int p2)
    {
        if ((nint)_validate_name == 0) throw new System.EntryPointNotFoundException("'validate_name' is not available for the loaded IDA SDK version.");
        return _validate_name(p0, p1, p2);
    }

    public static byte @value_repr_t__from_opinfo(void* p0, ulong p1, uint p2, void* p3, void* p4)
    {
        if ((nint)_value_repr_t__from_opinfo == 0) throw new System.EntryPointNotFoundException("'value_repr_t__from_opinfo' is not available for the loaded IDA SDK version.");
        return _value_repr_t__from_opinfo(p0, p1, p2, p3, p4);
    }

    public static byte @value_repr_t__parse_value_repr(void* p0, QString* p1, byte p2)
    {
        if ((nint)_value_repr_t__parse_value_repr == 0) throw new System.EntryPointNotFoundException("'value_repr_t__parse_value_repr' is not available for the loaded IDA SDK version.");
        return _value_repr_t__parse_value_repr(p0, p1, p2);
    }

    public static nuint @value_repr_t__print_(void* p0, QString* p1, byte p2)
    {
        if ((nint)_value_repr_t__print_ == 0) throw new System.EntryPointNotFoundException("'value_repr_t__print_' is not available for the loaded IDA SDK version.");
        return _value_repr_t__print_(p0, p1, p2);
    }

    public static int @vcred_ask_user(void* p0, void* p1, QString* p2, uint p3)
    {
        if ((nint)_vcred_ask_user == 0) throw new System.EntryPointNotFoundException("'vcred_ask_user' is not available for the loaded IDA SDK version.");
        return _vcred_ask_user(p0, p1, p2, p3);
    }

    public static byte @vcred_do_load_password(void* p0, QString* p1, QString* p2)
    {
        if ((nint)_vcred_do_load_password == 0) throw new System.EntryPointNotFoundException("'vcred_do_load_password' is not available for the loaded IDA SDK version.");
        return _vcred_do_load_password(p0, p1, p2);
    }

    public static byte @vcred_do_load_proxy_password(void* p0, QString* p1, QString* p2)
    {
        if ((nint)_vcred_do_load_proxy_password == 0) throw new System.EntryPointNotFoundException("'vcred_do_load_proxy_password' is not available for the loaded IDA SDK version.");
        return _vcred_do_load_proxy_password(p0, p1, p2);
    }

    public static void @vcred_init(void* p0)
    {
        if ((nint)_vcred_init == 0) throw new System.EntryPointNotFoundException("'vcred_init' is not available for the loaded IDA SDK version.");
        _vcred_init(p0);
    }

    public static byte @vcred_load_site(void* p0)
    {
        if ((nint)_vcred_load_site == 0) throw new System.EntryPointNotFoundException("'vcred_load_site' is not available for the loaded IDA SDK version.");
        return _vcred_load_site(p0);
    }

    public static byte @vcred_process_switch(void* p0, byte* p1)
    {
        if ((nint)_vcred_process_switch == 0) throw new System.EntryPointNotFoundException("'vcred_process_switch' is not available for the loaded IDA SDK version.");
        return _vcred_process_switch(p0, p1);
    }

    public static byte @vcred_reg_del_auto_connect()
    {
        if ((nint)_vcred_reg_del_auto_connect == 0) throw new System.EntryPointNotFoundException("'vcred_reg_del_auto_connect' is not available for the loaded IDA SDK version.");
        return _vcred_reg_del_auto_connect();
    }

    public static byte @vcred_reg_del_store_info()
    {
        if ((nint)_vcred_reg_del_store_info == 0) throw new System.EntryPointNotFoundException("'vcred_reg_del_store_info' is not available for the loaded IDA SDK version.");
        return _vcred_reg_del_store_info();
    }

    public static void @vcred_reg_set_auto_connect(byte p0)
    {
        if ((nint)_vcred_reg_set_auto_connect == 0) throw new System.EntryPointNotFoundException("'vcred_reg_set_auto_connect' is not available for the loaded IDA SDK version.");
        _vcred_reg_set_auto_connect(p0);
    }

    public static void @vcred_reg_set_site(void* p0, byte* p1)
    {
        if ((nint)_vcred_reg_set_site == 0) throw new System.EntryPointNotFoundException("'vcred_reg_set_site' is not available for the loaded IDA SDK version.");
        _vcred_reg_set_site(p0, p1);
    }

    public static void @vcred_reg_set_store_info(byte p0)
    {
        if ((nint)_vcred_reg_set_store_info == 0) throw new System.EntryPointNotFoundException("'vcred_reg_set_store_info' is not available for the loaded IDA SDK version.");
        _vcred_reg_set_store_info(p0);
    }

    public static byte @vcred_reg_should_auto_connect()
    {
        if ((nint)_vcred_reg_should_auto_connect == 0) throw new System.EntryPointNotFoundException("'vcred_reg_should_auto_connect' is not available for the loaded IDA SDK version.");
        return _vcred_reg_should_auto_connect();
    }

    public static byte @vcred_reg_should_store_info()
    {
        if ((nint)_vcred_reg_should_store_info == 0) throw new System.EntryPointNotFoundException("'vcred_reg_should_store_info' is not available for the loaded IDA SDK version.");
        return _vcred_reg_should_store_info();
    }

    public static byte @vcred_write(void* p0, QString* p1)
    {
        if ((nint)_vcred_write == 0) throw new System.EntryPointNotFoundException("'vcred_write' is not available for the loaded IDA SDK version.");
        return _vcred_write(p0, p1);
    }

    public static int @verify_argloc(void* p0, int p1, void* p2)
    {
        if ((nint)_verify_argloc == 0) throw new System.EntryPointNotFoundException("'verify_argloc' is not available for the loaded IDA SDK version.");
        return _verify_argloc(p0, p1, p2);
    }

    public static int @verify_tinfo(ulong p0)
    {
        if ((nint)_verify_tinfo == 0) throw new System.EntryPointNotFoundException("'verify_tinfo' is not available for the loaded IDA SDK version.");
        return _verify_tinfo(p0);
    }

    public static nint @visit_edms(TypeInfo* p0, ulong p1, int p2, byte p3, void* p4)
    {
        if ((nint)_visit_edms == 0) throw new System.EntryPointNotFoundException("'visit_edms' is not available for the loaded IDA SDK version.");
        return _visit_edms(p0, p1, p2, p3, p4);
    }

    public static int @visit_stroff_udms(void* p0, ulong* p1, int p2, long* p3, byte p4)
    {
        if ((nint)_visit_stroff_udms == 0) throw new System.EntryPointNotFoundException("'visit_stroff_udms' is not available for the loaded IDA SDK version.");
        return _visit_stroff_udms(p0, p1, p2, p3, p4);
    }

    public static int @visit_subtypes(void* p0, void* p1, TypeInfo* p2, byte* p3, byte* p4)
    {
        if ((nint)_visit_subtypes == 0) throw new System.EntryPointNotFoundException("'visit_subtypes' is not available for the loaded IDA SDK version.");
        return _visit_subtypes(p0, p1, p2, p3, p4);
    }

    public static byte* @winerr(int p0)
    {
        if ((nint)_winerr == 0) throw new System.EntryPointNotFoundException("'winerr' is not available for the loaded IDA SDK version.");
        return _winerr(p0);
    }

    public static void @write_struc_path(ulong p0, int p1, ulong* p2, int p3, long p4)
    {
        if ((nint)_write_struc_path == 0) throw new System.EntryPointNotFoundException("'write_struc_path' is not available for the loaded IDA SDK version.");
        _write_struc_path(p0, p1, p2, p3, p4);
    }

    public static ulong @write_tinfo_bitfield_value(ulong p0, ulong p1, ulong p2, int p3)
    {
        if ((nint)_write_tinfo_bitfield_value == 0) throw new System.EntryPointNotFoundException("'write_tinfo_bitfield_value' is not available for the loaded IDA SDK version.");
        return _write_tinfo_bitfield_value(p0, p1, p2, p3);
    }

    public static int @writebytes(int p0, uint p1, int p2, byte p3)
    {
        if ((nint)_writebytes == 0) throw new System.EntryPointNotFoundException("'writebytes' is not available for the loaded IDA SDK version.");
        return _writebytes(p0, p1, p2, p3);
    }

    public static byte @xrefblk_t_first_from(void* p0, ulong p1, int p2)
    {
        if ((nint)_xrefblk_t_first_from == 0) throw new System.EntryPointNotFoundException("'xrefblk_t_first_from' is not available for the loaded IDA SDK version.");
        return _xrefblk_t_first_from(p0, p1, p2);
    }

    public static byte @xrefblk_t_first_to(void* p0, ulong p1, int p2)
    {
        if ((nint)_xrefblk_t_first_to == 0) throw new System.EntryPointNotFoundException("'xrefblk_t_first_to' is not available for the loaded IDA SDK version.");
        return _xrefblk_t_first_to(p0, p1, p2);
    }

    public static byte @xrefblk_t_next_from(void* p0)
    {
        if ((nint)_xrefblk_t_next_from == 0) throw new System.EntryPointNotFoundException("'xrefblk_t_next_from' is not available for the loaded IDA SDK version.");
        return _xrefblk_t_next_from(p0);
    }

    public static byte @xrefblk_t_next_to(void* p0)
    {
        if ((nint)_xrefblk_t_next_to == 0) throw new System.EntryPointNotFoundException("'xrefblk_t_next_to' is not available for the loaded IDA SDK version.");
        return _xrefblk_t_next_to(p0);
    }

    public static byte @xrefchar(byte p0)
    {
        if ((nint)_xrefchar == 0) throw new System.EntryPointNotFoundException("'xrefchar' is not available for the loaded IDA SDK version.");
        return _xrefchar(p0);
    }

}
