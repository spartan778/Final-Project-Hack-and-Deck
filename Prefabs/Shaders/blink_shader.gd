extends Node

@export var healthSystem : HealthSystem
@export var shaderMat: ShaderMaterial
@export var animated_sprite: AnimatedSprite2D
@export var sprite_2d: Sprite2D

@export var duration:float
@export var interval:float
var is_shader_active: bool

@onready var duration_timer = $DurationTimer
@onready var interval_timer = $IntervalTimer

func _ready() -> void:
	duration_timer.wait_time = duration
	interval_timer.wait_time = interval
	is_shader_active = false
	
func _on_health_system_damage_effect(oldHealth: float, newHealth: float) -> void:
	print("old health: ", oldHealth, "\n new health: ", newHealth)
	if(animated_sprite):
		animated_sprite.material = shaderMat
		duration_timer.start()
		interval_timer.start()
	


func _on_interval_timer_timeout() -> void:
	_toggle_shader()
	
	
func _toggle_shader() -> void:
	if(is_shader_active):
		animated_sprite.material = null
		is_shader_active = false
	else:
		animated_sprite.material = shaderMat
		is_shader_active = true


func _on_duration_timer_timeout() -> void:
	interval_timer.stop()
	animated_sprite.material = null # making sure shader is disabled
