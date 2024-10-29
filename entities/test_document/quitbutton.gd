extends Button

func _on_pressed() -> void:
	get_parent().get_parent().get_parent().queue_free()


func _on_rich_text_label_meta_clicked(meta):
	print(meta)