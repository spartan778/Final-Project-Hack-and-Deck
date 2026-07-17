shader_type canvas_item;
uniform vec3 color_mod: source_color = vec3 (.3,.3,.3);
uniform float alpha = 1;


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	//COLOR = vec4(color_mod, COLOR.a * alpha);
	COLOR.rgb += color_mod;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
