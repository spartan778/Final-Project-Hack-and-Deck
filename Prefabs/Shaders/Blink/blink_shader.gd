extends Node

@export var healthSystem : HealthSystem
@export var shaderMat: ShaderMaterial
@export var animated_sprite: AnimatedSprite2D
@export var sprite_2d: Sprite2D
@export var duration:float
@export var interval:float

var target_sprite: CanvasItem

@onready var duration_timer = $DurationTimer
@onready var interval_timer = $IntervalTimer

func _ready() -> void:
	duration_timer.wait_time = duration
	interval_timer.wait_time = interval
	target_sprite = animated_sprite if animated_sprite else sprite_2d #assign either sprite2d or animatedSprite as the target
	
func _on_health_system_damage_effect(oldHealth: float, newHealth: float) -> void:
	print("old health: ", oldHealth, "\n new health: ", newHealth)
	if(!target_sprite): return #return if there is no targeted sprite
	duration_timer.start()
	interval_timer.start()

func _on_interval_timer_timeout() -> void:
	if(target_sprite.material == shaderMat):
		target_sprite.material = null
	else:
		target_sprite.material = shaderMat
	

func _on_duration_timer_timeout() -> void:
	interval_timer.stop()
	target_sprite.material = null # making sure shader is disabled


func _on_health_system_healing_effect(healedValue: float) -> void:
	if(!target_sprite): return #return if there is no targeted sprite
	duration_timer.start()
	interval_timer.start()
