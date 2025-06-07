using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TreeAPI.Application.Abstractions;
using TreeAPI.Dto;
using TreeAPI.Filters;
using TreeAPI.Mapping;

namespace TreeAPI.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Produces("application/json")]
    public class TreeController : ControllerBase
    {
        private readonly ILogger<TreeController> _logger;
        private readonly ITreeService _treeService;

        public TreeController(ILogger<TreeController> logger, ITreeService treeService)
        {
            _logger = logger;
            _treeService = treeService;
        }

        /// <summary>
        /// Returns your entire tree. If your tree doesn't exist it will be created automatically. 
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the tree <see cref="MNode"/>
        /// Returns a 200 OK status with the tree if successful
        /// </returns>
        [HttpPost]
        [Route("get")]
        [ProducesResponseType(typeof(string), 200)]
        [SwaggerOperation(
            Summary = "",
            Description = "Returns an entire tree. If your tree doesn't exist it will be created automatically.",
            Tags = new[] { "user.tree" }
            )]
        public async Task<IActionResult> Get([FromQuery, Required] string treeName, CancellationToken cancellationToken)
        {
            var tree = await _treeService.GetTreeByNameAsycn(treeName, cancellationToken);

            if (tree == null)
            {
                var result = _treeService.CreateTreeAsync(treeName, cancellationToken);
                if (!result.IsFaulted)
                {
                    return Ok("Tree created. Successfull operation.");
                }
                return BadRequest();
            }            

            var nodes = await _treeService.GetTreeNodesAsync(treeName, cancellationToken);

            return Ok(TreeMapper.MapToTreeResponse(tree, nodes));
        }

        /// <summary>
        /// Create a new node in your tree.
        /// You must to specify a parent node ID that belongs to your tree.
        /// A new node name must be unique across all siblings.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("node-create")]
        [ProducesResponseType(typeof(string), 200)]
        [SwaggerOperation(
            Summary = "",
            Description = "",
            Tags = new[] { "user.tree.node" }
            )]
        public async Task<IActionResult> Create(
            [FromQuery, Required] string treeName,
            [FromQuery, Required] long parentNodeId,
            [FromQuery, Required] string nodeName,
            CancellationToken cancellationToken)
        {
            await _treeService.CreateNodeAsync(treeName, parentNodeId, nodeName, cancellationToken);
            return Ok("Successful response");
        }

        /// <summary>
        /// Delete an existing node in your tree. You must specify a node ID that belongs your tree.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("node-delete")]
        [ProducesResponseType(typeof(string), 200)]
        [SwaggerOperation(
            Summary = "",
            Description = "Delete an existing node in your tree. You must specify a node ID that belongs your tree.",
            Tags = new[] { "user.tree.node" }
            )]
        public async Task<IActionResult> Delete(
            [FromQuery, Required] string treeName,
            [FromQuery, Required] long nodeId,
            CancellationToken cancellationToken)
        {
            await _treeService.DeleteNodeAsync(treeName, nodeId, cancellationToken);
            return Ok("Successful response");
        }

        /// <summary>
        /// Rename an existing node in your tree.
        /// You must specify a node ID that belongs your tree.
        /// A new name of the node must be unique across all siblings.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("node-rename")]
        [ProducesResponseType(typeof(string), 200)]
        [SwaggerOperation(
            Summary = "",
            Description = "Rename an existing node in your tree. You must specify a node ID that belongs your tree. A new name of the node must be unique across all siblings.",
            Tags = new[] { "user.tree.node" }
            )]
        public async Task<IActionResult> Rename(
            [FromQuery, Required] string treeName,
            [FromQuery, Required] long nodeId,
            [FromQuery, Required] string newNodeName,
            CancellationToken cancellationToken)
        {
            await _treeService.RenameNodeAsync(treeName, nodeId, newNodeName, cancellationToken);
            return Ok("Successful response");
        }
    }
}
